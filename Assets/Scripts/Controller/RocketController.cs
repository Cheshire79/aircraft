using Aircraft.Tools;
using Aircraft.View;
using Holoville.HOTween;
using UnityEngine;

namespace Aircraft.Controller
{
	public class RocketController
	{
		private RocketView _rocketView;
		private Vector2? _leaserTargetPosition;
		private bool _isFired;

		public Vector2 Position
		{
			get
			{
				return _rocketView.transform.localPosition;
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
				MainThreadRunner.AddTask(() =>
				{
					HOTween.Kill(_rocketView.transform);

					var velocity = new Vector3(_leaserTargetPosition.Value.x, _leaserTargetPosition.Value.y, 0) - _rocketView.transform.position;
					_rocketView.transform.rotation = Quaternion.LookRotation(velocity, Vector3.forward);					
					float distanceToTarget = Mathf.Sqrt(Mathf.Pow((_rocketView.transform.localPosition.x - _leaserTargetPosition.Value.x), 2) +
									  Mathf.Pow((_rocketView.transform.localPosition.y - _leaserTargetPosition.Value.y), 2));

					float timeToTarget = distanceToTarget / Constants.RocketVelocity;
					//Debug.LogWarning("distanceToTarget= " + distanceToTarget + "  timeToTarget  " + timeToTarget);
				

					HOTween.To(_rocketView.transform, timeToTarget / Constants.TimeScale,
			new TweenParms().Prop("localPosition", new Vector3(_leaserTargetPosition.Value.x, _leaserTargetPosition.Value.y, 0)).Ease(EaseType.Linear));
				});

			}
		}

		public void Fire(Transform abj)
		{
			_isFired = true;
			if (_leaserTargetPosition != null)
				MainThreadRunner.AddTask(() =>
				{
					
					_rocketView.transform.localPosition = new Vector3(0, 0, 0);

					float distanceToTarget = Mathf.Sqrt(Mathf.Pow((_rocketView.transform.localPosition.x - _leaserTargetPosition.Value.x), 2) +
									  Mathf.Pow((_rocketView.transform.localPosition.y - _leaserTargetPosition.Value.y), 2));

					float timeToTarget = distanceToTarget / Constants.RocketVelocity;
					Debug.LogWarning("distanceToTarget= " + distanceToTarget + "  timeToTarget  " + timeToTarget);
					HOTween.Kill(_rocketView.transform);
				
					Vector3 velocity = new Vector3(_leaserTargetPosition.Value.x, _leaserTargetPosition.Value.y, 0) - _rocketView.transform.position;
					_rocketView.transform.rotation = Quaternion.LookRotation(velocity, Vector3.forward);
					_rocketView.gameObject.SetActive(true);
					
					HOTween.To(_rocketView.transform, timeToTarget / Constants.TimeScale,
			new TweenParms().Prop("localPosition", new Vector3(_leaserTargetPosition.Value.x, _leaserTargetPosition.Value.y, 0)).Ease(EaseType.Linear));
				});
		}
	
		public void Stop()
		{
			HOTween.Kill(_rocketView.transform);
			_rocketView.transform.localPosition = new Vector3(0, 0, 0);
			_rocketView.gameObject.SetActive(false);
			_isFired = false;
		}

		public bool IsHitting(Vector2 TargetPosition)
		{
			if (_isFired)
			{
				float distanceToTarget = Mathf.Sqrt(Mathf.Pow((_rocketView.transform.localPosition.x - TargetPosition.x), 2) +
									  Mathf.Pow((_rocketView.transform.localPosition.y - TargetPosition.y), 2));
				//Debug.LogWarning("distanceToTarget= " + distanceToTarget);
				if (distanceToTarget < 0.1f)
					return true;
			}
			return false;
		}

	}
}
