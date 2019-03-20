using Aircraft.Tools.Trajectory;
using System;
using UnityEngine;

namespace Aircraft.Tools.Trajectory
{
	public class SomeTrajectory : ITrajectory
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
		public SomeTrajectory(Vector2 screenSize, System.Random rng)
		{
			StartPoint = new Vector2(screenSize.x / 2 - rng.Next((int)screenSize.x), screenSize.y / 2 - rng.Next((int)screenSize.y));
			_currentPoint = StartPoint;
			_maxVelocity = Constants.AircraftVelocityMin +
				rng.Next((int)(Constants.AircraftVelocityMax - Constants.AircraftVelocityMin));
			_vX = rng.Next((int)(2 * _maxVelocity * 100)) / 100.0f - _maxVelocity;
		}

		public Vector2 GenerateCoord()
		{
			_vX -= 0.01f;
			if (_vX < -Constants.AircraftVelocityMax - 0.1)
				_vX = Constants.AircraftVelocityMin;
			_vY = (float)Math.Sqrt((Constants.AircraftVelocityMin * Constants.AircraftVelocityMin) - (_vX * _vX));

			_currentPoint.x += _vX * Constants.RepaintInterval * Constants.TimeScale / 1000.0f;
			_currentPoint.y += _vY * Constants.RepaintInterval * Constants.TimeScale / 1000.0f;

			return _currentPoint;
		}
	}
}
