using UnityEngine;

namespace Quinn
{
	/// <summary>
	/// A static utility class for randomness.
	/// </summary>
	public static class RandomUtility
	{
		public static Vector2 GetRandomDirection(float diameter = 1f)
		{
			float theta = Random.Range(0f, Mathf.PI * 2f);
			return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * diameter;
		}
	}
}
