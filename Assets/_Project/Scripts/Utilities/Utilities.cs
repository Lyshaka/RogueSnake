using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class Utilities
{
	public static Transform GetNearest(Vector3 worldPosition, List<Transform> list)
	{
		if (list == null || list.Count == 0) return null; // Return null if the list is null or empty
		if (list.Count == 1) return list[0]; // Return the first element if the list only contains one element

		float shortestDistance = float.PositiveInfinity;
		Transform nearest = null;

		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == null) continue; // If this transform is null we just skip it

			float currentDistance = (list[i].position - worldPosition).sqrMagnitude; // Calculate the squared distance between the two positions
			if (currentDistance < shortestDistance) // Compare it to the current shortest distance we have
			{
				// If it is we just update the shortest distance and nearest current object
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

	public static Vector3 GridToWorld(Vector2Int gridPos)
	{
		return new(gridPos.x, 0f, gridPos.y);
	}

	public static Vector2Int WorldToGrid(Vector3 worldPos)
	{
		return new((int)worldPos.x, (int)worldPos.z);
	}

	public static string TimeToString(float totalSeconds)
	{
		int hours = (int)(totalSeconds / 3600);
		int minutes = (int)((totalSeconds % 3600) / 60);
		int seconds = (int)(totalSeconds % 60);
		int milliseconds = (int)((totalSeconds - MathF.Floor(totalSeconds)) * 1000);

		return $"{hours:00}:{minutes:00}:{seconds:00}:{milliseconds:000}";
	}

	public static string TimeToString(float totalSeconds, int spacing)
	{
		int hours = (int)(totalSeconds / 3600);
		int minutes = (int)((totalSeconds % 3600) / 60);
		int seconds = (int)(totalSeconds % 60);
		int milliseconds = (int)((totalSeconds - MathF.Floor(totalSeconds)) * 1000);

		return $"<mspace={spacing}>{hours:00}</mspace>:<mspace={spacing}>{minutes:00}</mspace>:<mspace={spacing}>{seconds:00}</mspace>:<mspace={spacing}>{milliseconds:000}</mspace>";
	}

	public static string Parse(object input, ParseType type, int spacing = -1)
	{
		switch (type)
		{
			case ParseType.Time:
				if (spacing < 0)
					return TimeToString((float)input);
				else
					return TimeToString((float)input, spacing);
			case ParseType.Integer:
				return $"{input}";
			case ParseType.Percentage:
				if (input is string str)
					return $"{(float.Parse(str, CultureInfo.InvariantCulture) * 100f):0.0#}%";
				else if (input is float f)
					return $"{(f * 100f):0.0#}%";
				return "";
			case ParseType.Text:
				return (string)input;
			default:
				return "";
			}
	}


	public enum ParseType
	{
		Time,
		Integer,
		Percentage,
		Text,
	}

}
