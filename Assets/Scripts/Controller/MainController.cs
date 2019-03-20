using Aircraft.Tools;
using Aircraft.Tools.GUI;
using Aircraft.View;
using UnityEngine;
using UnityEngine.UI;

namespace Aircraft.Controller
{
	public class MainController : MonoBehaviour
	{

		public RectTransform RectTransform;
		public ButtonAdv LaunchAircraftButton;

		public AircraftView AircraftViewPrefab;
		public AirDefense AirDefensePrefab;
		private AirDefense _airDefense;

		public RocketView RocketViewPrefab;

		public LeadingTargetView LeadingTargetView;
		private LeadingTargetView _leadingTarget;
		public Transform GraphicElementsHolder;
		private Vector2 _screenSize;
		private Vector2? _aircraftOLdPosition;
		private AircraftController _aircraftController;
		private RocketController _rocketController;

		
		public void Init(Camera camera)
		{
			LaunchAircraftButton.interactable = false;
			LaunchAircraftButton.onClick.AddListener(OnLaunchAircraftButtonClicked);

			_screenSize = new Vector2(RectTransform.rect.width / GraphicElementsHolder.transform.localScale.x, RectTransform.rect.height / GraphicElementsHolder.transform.localScale.y);
		
			_aircraftController = new AircraftController(AircraftViewPrefab, GraphicElementsHolder, _screenSize); // todo DI
			_aircraftController.ChangePosition += OnAircraftChangePosition;

			_rocketController = new RocketController(RocketViewPrefab, GraphicElementsHolder);


			LaunchAircraftButton.interactable = true;
			_aircraftOLdPosition = null;
		

			_leadingTarget = MonoBehaviour.Instantiate(LeadingTargetView);
			_leadingTarget.transform.parent = GraphicElementsHolder;
			_leadingTarget.transform.localScale = new Vector3(1 / GraphicElementsHolder.transform.localScale.x, 1 / GraphicElementsHolder.transform.localScale.x, 1 / GraphicElementsHolder.transform.localScale.x);
			_leadingTarget.transform.localPosition = new Vector3(0, 0, 1);// Vector3.one;
			_leadingTarget.transform.name = "leadingTarget";


			_airDefense = MonoBehaviour.Instantiate(AirDefensePrefab);
			_airDefense.transform.parent = GraphicElementsHolder;
			_airDefense.transform.localScale = new Vector3(1 / GraphicElementsHolder.transform.localScale.x, 1 / GraphicElementsHolder.transform.localScale.x, 1 / GraphicElementsHolder.transform.localScale.x);
			_airDefense.transform.localPosition = new Vector3(0, 0, 0);// Vector3.one;
			_airDefense.transform.name = "airDefense";
			_airDefense.Init();
			_airDefense.Fire += OnFire;

			_aircraftController.Run();

		}

		private void OnAircraftChangePosition(Vector2 pos)
		{
			MainThreadRunner.AddTask(() =>
			{
				if (pos.x > -_screenSize.x / 2 && pos.x < _screenSize.x / 2
				 && pos.y > -_screenSize.y / 2 && pos.y < _screenSize.y / 2)
				{
					if (_aircraftOLdPosition != null)
					// can figure out aircraft velosity and so on
					{
						if (_rocketController.IsHitting(pos))
						{
							OnHitting();
						}
						else
						{
							//Debug.LogWarning(" pos x = " + pos.x + " y = " + pos.y + "  _rocketPos  " + _rocketController.Position.x + " " + _rocketController.Position.y);
							Vector2 LeaserTargetPosition = FigureOutLeaserTarget(pos, new Vector2(_aircraftOLdPosition.Value.x, _aircraftOLdPosition.Value.y), _rocketController.Position);
							_rocketController.OnLeaserTargetPositionChange(LeaserTargetPosition);
							_aircraftOLdPosition = pos;
							_leadingTarget.transform.gameObject.SetActive(true);
						}
					}
					else
						_aircraftOLdPosition = pos;
				}
				else
				{
					OnLeftScreen();
				}
			});
		}


		private void OnLaunchAircraftButtonClicked()
		{
			if (_aircraftOLdPosition == null)
			{
				_aircraftController.Run();
			}
			else
			{
				_aircraftController.Stop();
				_rocketController.Stop();
			}

		}
		public void OnExit()
		{
			_aircraftController.Stop();
			_rocketController.Stop();
		}

		private void OnLeftScreen()
		{
			Debug.LogWarning("Has  leaft screen");
			_aircraftOLdPosition = null;
			_aircraftController.Stop();
			_rocketController.Stop();
			_aircraftController.Run();
			
			_airDefense.FireButton.interactable = true;
			_leadingTarget.transform.gameObject.SetActive(false);
		}

		private void OnHitting()
		{
			Debug.LogWarning("Aircraft was hitted");
			_aircraftOLdPosition = null;
			_aircraftController.Stop();			
			_rocketController.Stop();
			_airDefense.FireButton.interactable = true;
			_aircraftController.Run();

			_leadingTarget.transform.gameObject.SetActive(false);

		}

		private Vector2 FigureOutLeaserTarget(Vector2 aircraftPos, Vector2 aircraftOldPos, Vector2 rocketPos)
		{

			float koef = (Constants.RepaintInterval / 1000.0f) * Constants.TimeScale;
			float aircraftVelocity = Mathf.Sqrt(Mathf.Pow(((aircraftOldPos.x - aircraftPos.x) / (koef)), 2) + Mathf.Pow(((aircraftOldPos.y - aircraftPos.y) / (koef)), 2));


			Vector2 vectorFromRocket = aircraftPos - rocketPos;
			Vector2 vectorAircraft = aircraftPos - aircraftOldPos;

			float angle = -AngleBetweenVector(vectorAircraft, vectorFromRocket); 

			float a = aircraftVelocity / Constants.RocketVelocity;
			float timeToGetTarget;
			float distanceForTarget =
				 Mathf.Sqrt(Mathf.Pow(((aircraftPos.x - rocketPos.x)), 2) + Mathf.Pow(((aircraftPos.y - rocketPos.y)), 2));


			timeToGetTarget = distanceForTarget /
				(
				Constants.RocketVelocity * Mathf.Sqrt(1 - a * a * Mathf.Sin(angle * Mathf.PI / 180) * Mathf.Sin(angle * Mathf.PI / 180))
				- aircraftVelocity * Mathf.Cos(angle * Mathf.PI / 180)
				);

		//	Debug.LogWarning("_____________timeToGetTarget = " + timeToGetTarget + " distance  " + distanceForTarget);


			Vector2 delta = new Vector2
				(
				timeToGetTarget * aircraftVelocity * Mathf.Cos(angle * Mathf.PI / 180),
				timeToGetTarget * aircraftVelocity * Mathf.Sin(angle * Mathf.PI / 180)
				);

			vectorAircraft.Normalize();
			var vectorFromAircraftToLeaserTarget = vectorAircraft * timeToGetTarget * aircraftVelocity;

			MainThreadRunner.AddTask(() =>
			_leadingTarget.transform.localPosition = new Vector3(aircraftPos.x + vectorFromAircraftToLeaserTarget.x, aircraftPos.y + vectorFromAircraftToLeaserTarget.y, 0));
		//	Debug.LogWarning("_____________ vectorFromAircraftToLeaserTarget. = " + vectorFromAircraftToLeaserTarget.x + " y " + vectorFromAircraftToLeaserTarget.y);
			return new Vector2(aircraftPos.x + vectorFromAircraftToLeaserTarget.x, aircraftPos.y + vectorFromAircraftToLeaserTarget.y);
		}


		float AngleBetweenVector(Vector2 vec1, Vector2 vec2)
		{
			Vector2 vec1Rotated90 = new Vector2(-vec1.y, vec1.x);
			float sign = (Vector2.Dot(vec1Rotated90, vec2) < 0) ? -1.0f : 1.0f;
			return Vector2.Angle(vec1, vec2) * sign;
		}

		private void OnFire()
		{
			Debug.LogWarning(" Fired ");
			_rocketController.Fire(_leadingTarget.transform);
			_airDefense.FireButton.interactable = false;
		}
	}

}
