using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Aircraft.View
{
	public class RocketView : MonoBehaviour
	{
		
		//private Vector2 _startMarker;
		//private Vector2 _endMarker;

		//public float speed = 1.0F;
		//// Time when the movement started.
		//private float startTime;
		//// Total distance between the markers.
		//private float journeyLength;
		//private bool _isMoving = false;
		//private bool _toSAvePosition = false;
		//public void StartStep(Vector2 endMarker)
		//{
		//	_isMoving = true;
		//	_endMarker = endMarker;
		//	//_startMarker = transform.localPosition;
		//	//startMarker;

		//	// Keep a note of the time the movement started.
		//	//startTime = Time.time;
		//	startTime = 0;
		//	// Calculate the journey length.
		//	//	journeyLength = Vector2.Distance(_startMarker, _endMarker);
		//	CustomLogger.Log("_____________ distance ____________ = ++++++++++++++++++++++++++++++++++++++++++++++++++++");

		//	_toSAvePosition = true;
		//}

		//public void Stop()
		//{
		//	_isMoving = false;
		//	transform.localPosition = Vector3.zero;
		//	//startMarker;

		//	// Keep a note of the time the movement started.
		//	startTime = 0;// Time.time;
		//	startTime = Time.time;
		//	// Calculate the journey length.
		//	journeyLength = Vector2.Distance(_startMarker, _endMarker);


		//}
		//Vector3 _oldpos = Vector3.zero;
		//void FixedUpdate()
		//{

		//	if (_isMoving)
		//	{
		//		if (_toSAvePosition)
		//		{
		//			_toSAvePosition = false;
		//			startTime = 0;
		//			_startMarker = transform.localPosition;
		//			journeyLength = Vector2.Distance(_startMarker, _endMarker);
		//		}
		//		// Distance moved = time * speed.
		//		startTime += Time.fixedDeltaTime;
		//		float distCovered = (//Time.time -
		//			startTime) * Constants.RocketVelocity * Constants.TimeScale;

		//		// Fraction of journey completed = current distance divided by total distance.
		//		float fracJourney = distCovered / journeyLength;

		//		// Set our position as a fraction of the distance between the markers.

		//		var current = Vector3.Lerp(_startMarker, _endMarker, fracJourney);
		//		transform.localPosition = new Vector3(current.x, current.y, 0);
		//		var d = Vector3.Distance(_oldpos, transform.localPosition);

		//		CustomLogger.Log(
		//				  (_oldpos.x - transform.localPosition.x) + "_____________ distance ____________ = " + transform.localPosition.x + " " + _oldpos.x + " " + " distance " + d + "  " + (transform.localPosition.x - current.x));


		//		_oldpos = transform.localPosition;
		//	}
		//}
	}
}
