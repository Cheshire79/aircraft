using UnityEngine;

namespace Aircraft.Tools.Trajectory
{
	public interface ITrajectory
	{
		Vector2 StartPoint
		{
			get;
		}
		Vector2 GenerateCoord();
	}
}
