using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Aircraft.Tools.Trajectory
{
	public class CircleTrajectory : ITrajectory
	{
		private float _vX;
		private float _vY;
		private Vector2 _currentPoint;
		public Vector2 StartPoint
		{
			get;
			private set;
		}
		float _v;
		float _angle = 0;
		public CircleTrajectory(Vector2 screenSize, System.Random rng)
		{
			_v = Mathf.Sqrt(Constants.AircraftVelocityMax);

			 StartPoint = new Vector2(-7, 0);
			_currentPoint = StartPoint;
			_currentPoint = StartPoint;
			
			//	Debug.LogWarning(" AircraftController run vx" + _vY + "  vy" + _vY + "_maxVelocity" + _maxVelocity+ " " + Mathf.Sqrt((_vX * _vX) + (_vY * _vY)));
		}


		public Vector2 GenerateCoord()
		{

			Debug.LogWarning(" Aircraf" + _v * Mathf.Sin(_angle * Mathf.PI / 180) +" "+ _currentPoint.x);
			Debug.LogWarning(" Aircraf" + _v * Mathf.Cos(_angle * Mathf.PI / 180) + " " + _currentPoint.y);

			_currentPoint.x += _v * Mathf.Sin(_angle * Mathf.PI / 180) * Constants.PulseRepetitionInterval * Constants.TimeScale / 1000.0f;
			_currentPoint.y += _v * Mathf.Cos(_angle * Mathf.PI / 180) * Constants.PulseRepetitionInterval * Constants.TimeScale / 1000.0f;
			
			_angle += 1.50f;

			return _currentPoint;
		}
	}
}
