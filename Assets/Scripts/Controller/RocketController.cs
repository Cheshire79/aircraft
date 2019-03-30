using Aircraft.Tools;
using Aircraft.View;
using Holoville.HOTween;
using System;
using UnityEngine;

namespace Aircraft.Controller
{
	public class RocketController
	{
		private RocketView _rocketView;
		private Vector2? _leaserTargetPosition;
		private bool _isFired;
		private Vector2 _currentPosition;
		private object vectorREcket;
		private DateTime _timeOfPreviosPosition;
		public Vector2 Position
		{
			get
			{
				return _currentPosition;
			}
		}

		public RocketController(RocketView RocketViewPrefab, Transform graphicElementsHolder)
		{
			_leaserTargetPosition = null;
			_isFired = false;

			_rocketView = MonoBehaviour.Instantiate(RocketViewPrefab);
			_rocketView.transform.parent = graphicElementsHolder;
			_rocketView.transform.localScale = new Vector3(1 / graphicElementsHolder.transform.localScale.x, 1 / graphicElementsHolder.transform.localScale.x, 1 / graphicElementsHolder.transform.localScale.x);
			_rocketView.transform.localPosition = new Vector3(0, 0, 0);
			_rocketView.transform.name = "rocket";
			_rocketView.gameObject.SetActive(false);


		}

		public void OnLeaserTargetPositionChange(Vector2 pos)
		{
			_leaserTargetPosition = pos;
			if (_isFired)
			{
				// figure out the current rocket position
				TimeSpan span = (System.DateTime.Now - _timeOfPreviosPosition);
				int differInMs = (int)span.TotalMilliseconds;
				//Debug.LogWarning(" delta time= " + i);
				Vector2 vectorRocket = _leaserTargetPosition.Value - _currentPosition;
				vectorRocket.Normalize();
				_currentPosition += vectorRocket * Constants.RocketVelocity * differInMs / 1000 * Constants.TimeScale;
				_timeOfPreviosPosition = System.DateTime.Now;


				MainThreadRunner.AddTask(() =>
				{
					HOTween.Kill(_rocketView.transform);

					//https://answers.unity.com/questions/585035/lookat-2d-equivalent-.html
					var velocity = new Vector3(_leaserTargetPosition.Value.x, _leaserTargetPosition.Value.y, 0) - _rocketView.transform.localPosition;
					float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
					Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

					_rocketView.transform.localRotation = q;
					float distanceToTarget = Mathf.Sqrt(Mathf.Pow((_rocketView.transform.localPosition.x - _leaserTargetPosition.Value.x), 2) +
									  Mathf.Pow((_rocketView.transform.localPosition.y - _leaserTargetPosition.Value.y), 2));

					float timeToTarget = distanceToTarget / Constants.RocketVelocity;
					//Debug.LogWarning("distanceToTarget= " + distanceToTarget + "  timeToTarget  " + timeToTarget);

					HOTween.To(_rocketView.transform, timeToTarget / Constants.TimeScale,
				new TweenParms().Prop("localPosition", new Vector3(_leaserTargetPosition.Value.x, _leaserTargetPosition.Value.y, 0)).Ease(EaseType.Linear));

					//			HOTween.To(_rocketView.transform, Constants.PulseRepetitionInterval / 1000 ,
					//new TweenParms().Prop("localPosition", new Vector3(_currentPosition.x, _currentPosition.y, 0)).Ease(EaseType.Linear));
				});

			}
		}

		public void Fire(Transform obj)
		{
			_isFired = true;
			if (_leaserTargetPosition != null)
			{

				_currentPosition = new Vector3(0, 0, 0);
				Vector2 vectorRocket = _leaserTargetPosition.Value - _currentPosition;
				vectorRocket.Normalize();
				_currentPosition = vectorRocket * Constants.RocketVelocity * Constants.PulseRepetitionInterval / 1000 * Constants.TimeScale;
				_timeOfPreviosPosition = System.DateTime.Now; // for renew rocket position on  OnLeaserTargetPositionChange


				float distanceToTarget = Mathf.Sqrt(Mathf.Pow((_rocketView.transform.localPosition.x - _leaserTargetPosition.Value.x), 2) +
								  Mathf.Pow((_rocketView.transform.localPosition.y - _leaserTargetPosition.Value.y), 2));

				float timeToTarget = distanceToTarget / Constants.RocketVelocity;
				//Debug.LogWarning("distanceToTarget= " + distanceToTarget + "  timeToTarget  " + timeToTarget);
				MainThreadRunner.AddTask(() =>
			{
				_rocketView.transform.localPosition = new Vector3(0, 0, 0);
				HOTween.Kill(_rocketView.transform);

				Vector3 velocity = new Vector3(_leaserTargetPosition.Value.x, _leaserTargetPosition.Value.y, 0) - _rocketView.transform.position;
				_rocketView.transform.rotation = Quaternion.LookRotation(velocity, Vector3.forward);
				_rocketView.gameObject.SetActive(true);

				HOTween.To(_rocketView.transform, timeToTarget / Constants.TimeScale,
		  new TweenParms().Prop("localPosition", new Vector3(_leaserTargetPosition.Value.x, _leaserTargetPosition.Value.y, 0)).Ease(EaseType.Linear));

				//	HOTween.To(_rocketView.transform, Constants.PulseRepetitionInterval / 1000 ,
				//new TweenParms().Prop("localPosition", new Vector3(_currentPosition.x, _currentPosition.y, 0)).Ease(EaseType.Linear));
			});

			}
		}

		public void Stop()
		{
			_isFired = false;
			_currentPosition = Vector2.zero;
			MainThreadRunner.AddTask(() =>
			{
				HOTween.Kill(_rocketView.transform);
				_rocketView.transform.localPosition = new Vector3(0, 0, 0);
				_rocketView.gameObject.SetActive(false);

			});
		}

		public bool IsHitting(Vector2 TargetPosition)
		{
			if (_isFired)
			{

				TimeSpan span = (System.DateTime.Now - _timeOfPreviosPosition);
				int differInMs = (int)span.TotalMilliseconds;
				Vector2 vectorRocket = _leaserTargetPosition.Value - _currentPosition;
				vectorRocket.Normalize();
				var pos = _currentPosition + vectorRocket * Constants.RocketVelocity * differInMs / 1000 * Constants.TimeScale;
				// current rocket position - to check the distance between rocket and target
				float distanceToTarget = Mathf.Sqrt(Mathf.Pow((pos.x - TargetPosition.x), 2) + Mathf.Pow((pos.y - TargetPosition.y), 2));
				float precise = Constants.RocketVelocity * Constants.PulseRepetitionInterval * Constants.TimeScale / 1000;
				//	Debug.LogWarning("calc = " + pos.x + "  rocket  " + _rocketView.transform.localPosition.x+ "target " + TargetPosition.x+ " precise " + precise);			
				if (distanceToTarget < precise)
					return true;
			}
			return false;
		}

	}
}
