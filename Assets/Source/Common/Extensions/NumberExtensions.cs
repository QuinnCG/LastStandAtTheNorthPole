using UnityEngine;

namespace Quinn
{
	public static class NumberExtensions
	{
		public static float ToRadians(this float degrees)
		{
			return Mathf.Deg2Rad * degrees;
		}
		public static float ToDegrees(this float radians)
		{
			return Mathf.Rad2Deg * radians;
		}
		
		public static Vector2 ToDirection(this float degrees)
		{
			return new(Mathf.Cos(degrees), Mathf.Sin(degrees));
		}

		/// <summary>
		/// <c>Mathf.Abs(x - y)</c>
		/// </summary>
		public static float AbsDiff(this float x, float y)
		{
			return Mathf.Abs(x - y);
		}

		/// <remarks>
		/// This does not return a copy. This modifies the value directly.
		/// </remarks>
		/// <param name="min">Limit the value so that it is no lower than this.</param>
		public static void MakeAtLeast(this ref float value, float min)
		{
			value = Mathf.Max(value, min);
		}

		public static void MakeLessThan(this ref float value, float max)
		{
			value = Mathf.Min(value, max);
		}

		public static float Max(this ref float x, float y)
		{
			return Mathf.Max(x, y);
		}

		public static float Min(this ref float x, float y)
		{
			return Mathf.Min(x, y);
		}

		public static bool IsEven(this int value)
		{
			return value % 2 == 0;
		}

		public static bool IsOdd(this int value)
		{
			return value % 2 > 0;
		}

		public static bool NearlyEqual(this float a, float b, float tolerance = float.Epsilon)
		{
			return Mathf.Abs(a - b) <= tolerance;
		}

		public static short ToShort(this int v)
		{
			return (short)v;
		}

		public static float MapRange(this float value, float fromMin, float fromMax, float toMin, float toMax)
		{
			float dstA = fromMax - fromMin;
			float dstB = toMax - toMin;

			float offset = toMin - fromMin;
			value += offset;

			float scale = dstB / dstA;
			value *= scale;

			return value;
		}

		public static float Map01(this float value, float min, float max)
		{
			return value.MapRange(min, max, 0f, 1f);
		}

		/// <summary>
		/// Maps trig functions that range from -1 to 1 into a range of 0 to 1.
		/// </summary>
		public static float MapTrig01(this float value)
		{
			return value.MapRange(-1f, 1f, 0f, 1f);
		}
	}
}
