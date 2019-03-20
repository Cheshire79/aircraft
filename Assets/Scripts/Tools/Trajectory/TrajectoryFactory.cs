using System;
using UnityEngine;

namespace Aircraft.Tools.Trajectory
{
	public class TrajectoryFactory
	{
		private static System.Random _randomnGenerator = new System.Random(); // static for thread safety
		private Vector2 _screenSize;
		public TrajectoryFactory(Vector2 screenSize)
		{
			_screenSize = screenSize;
		}
		public ITrajectory CreateTrajectory()
		{
			return new LInierTrajectory(_screenSize, _randomnGenerator);
			int choice = _randomnGenerator.Next(2);
			switch (choice)
			{
				case 0:
					Debug.LogWarning("create linier trajectory");
					return new LInierTrajectory(_screenSize, _randomnGenerator);
					break;
				case 1:
					Debug.LogWarning("create none linier trajectory");
					return new SomeTrajectory(_screenSize, _randomnGenerator);
				default:
					Debug.LogWarning("create none linier trajectory");
					return new SomeTrajectory(_screenSize, _randomnGenerator);					
			}

		}
	}

}
