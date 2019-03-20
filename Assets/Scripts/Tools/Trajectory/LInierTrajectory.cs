using System;
using UnityEngine;

namespace Aircraft.Tools.Trajectory
{
	public class LInierTrajectory : ITrajectory
	{
		private float _vX;
		private float _vY;
		private float _maxVelocity;
		private Vector2 _currentPoint;
		public Vector2 StartPoint
		{
			get;
			private set;
		}
		public LInierTrajectory(Vector2 screenSize, System.Random rng)
		{
			StartPoint = new Vector2(screenSize.x / 2 - rng.Next((int)screenSize.x), screenSize.y / 2 - rng.Next((int)screenSize.y));
			_currentPoint = StartPoint;
			_maxVelocity = Constants.AircraftVelocityMin +
				rng.Next((int)(Constants.AircraftVelocityMax - Constants.AircraftVelocityMin));
			_vX = rng.Next((int)(2 * _maxVelocity * 100)) / 100.0f - _maxVelocity;
			_vY = (float)Math.Sqrt((_maxVelocity * _maxVelocity) - (_vX * _vX));
			//	Debug.LogWarning(" AircraftController run vx" + _vY + "  vy" + _vY + "_maxVelocity" + _maxVelocity+ " " + Mathf.Sqrt((_vX * _vX) + (_vY * _vY)));
		}


		public Vector2 GenerateCoord()
		{
			_currentPoint.x += _vX * Constants.RepaintInterval * Constants.TimeScale / 1000.0f;
			_currentPoint.y += _vY * Constants.RepaintInterval * Constants.TimeScale / 1000.0f;


			return _currentPoint;
		}
	}
}
