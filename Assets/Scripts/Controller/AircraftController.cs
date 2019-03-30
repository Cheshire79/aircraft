using Aircraft.Tools;
using Aircraft.Tools.Trajectory;
using Aircraft.View;
using Holoville.HOTween;
using System;
using UnityEngine;

namespace Aircraft.Controller
{
	public class AircraftController
	{
		private AircraftView _aircraftView;
		private readonly System.Timers.Timer _scheduledTask = new System.Timers.Timer(Constants.PulseRepetitionInterval);
		private bool _isRunnig = false;
		private Vector2 _currentPoint;
		private Vector2 _screenSize;
		public Action<Vector2> ChangePosition;
		private ITrajectory _trajectory;
		private TrajectoryFactory _trajectoryFactory;

		public AircraftController(AircraftView aircraftViewPrefab, Transform graphicElementsHolder, Vector2 screenSize)
		{
			Debug.LogWarning(" AircraftController start");
			_screenSize = screenSize;
			_aircraftView = MonoBehaviour.Instantiate(aircraftViewPrefab);
			_aircraftView.transform.parent = graphicElementsHolder;
			_aircraftView.transform.localScale = new Vector3(1 / graphicElementsHolder.transform.localScale.x, 1 / graphicElementsHolder.transform.localScale.x, 1 / graphicElementsHolder.transform.localScale.x);
			_aircraftView.transform.name = "aircraft";
			_aircraftView.gameObject.SetActive(false);
			_scheduledTask.Elapsed += (sender, args) => Runnable();
			_trajectoryFactory = new TrajectoryFactory(_screenSize);
		}

		public void Run()
		{
			_trajectory = _trajectoryFactory.CreateTrajectory();
			_currentPoint = _trajectory.StartPoint;
			MainThreadRunner.AddTask(() =>
			{
				_aircraftView.gameObject.SetActive(true);
				_aircraftView.transform.localPosition = new Vector3(_currentPoint.x, _currentPoint.y, 0);
			}
			);
			_scheduledTask.Start();
		}

		public void Stop()
		{
			_scheduledTask.Stop();

			_isRunnig = false;
			MainThreadRunner.AddTask(() =>
			{
				_aircraftView.gameObject.SetActive(false);
				HOTween.Kill(_aircraftView.transform);
			});
		}

		private void Runnable()
		{
			if (!_isRunnig)
			{
				try
				{
					_isRunnig = true;
					Vector2 position = _trajectory.GenerateCoord();
					MainThreadRunner.AddTask(() =>
					{
						HOTween.Kill(_aircraftView.transform);
						var velocity = new Vector3(position.x, position.y, 0) - _aircraftView.transform.localPosition;
						float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
						Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
						_aircraftView.transform.rotation = q;
						//Debug.LogWarning("rotation" + _aircraftView.transform.rotation.x + " " + _aircraftView.transform.rotation.y + " " + _aircraftView.transform.rotation.z);
						HOTween.To(_aircraftView.transform, Constants.PulseRepetitionInterval / 1000.0f,
			new TweenParms().Prop("localPosition", new Vector3(position.x, position.y, 0)).Ease(EaseType.Linear));

					});
					if (ChangePosition != null)
						ChangePosition(position);
					_isRunnig = false;

				}
				catch (Exception e)
				{
					Debug.LogWarning(" AircraftController Runnable" + e.ToString());
				}
			}
		}
	}
}
