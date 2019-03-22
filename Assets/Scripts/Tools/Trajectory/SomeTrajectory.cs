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
		private int sign=-1;
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
			_vX += sign * 0.01f;
			if (Mathf.Abs(_vX) > Constants.AircraftVelocityMax )
			{
				sign *= -1;
				_vX += sign * 0.01f;
			}


			_vY = (float)Math.Sqrt((Constants.AircraftVelocityMin * Constants.AircraftVelocityMin) - (_vX * _vX));

			_currentPoint.x += _vX * Constants.PulseRepetitionInterval * Constants.TimeScale / 1000.0f;
			_currentPoint.y += _vY * Constants.PulseRepetitionInterval * Constants.TimeScale / 1000.0f;

			return _currentPoint;
		}
	}
}
