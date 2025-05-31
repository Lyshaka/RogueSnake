using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
	public static Transform GetNearest(Vector3 worldPosition, List<Transform> list)
	{
		if (list.Count == 0) return null;
		if (list.Count == 1) return list[0];

		float shortestDistance = float.PositiveInfinity;
		Transform nearest = null;

		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == null) continue;

			float currentDistance = (list[i].position - worldPosition).sqrMagnitude;
			if (currentDistance < shortestDistance)
			{
				shortestDistance = currentDistance;
				nearest = list[i];
			}
		}

		return nearest;
	}

	public static bool IsInRange(Vector3 origin, Vector3 destination, float range)
	{
		return (destination - origin).sqrMagnitude <= (range * range);
	}
}
