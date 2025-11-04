using Unity.Mathematics;

namespace Quinn
{
	public static class NativeTypeExtensions
	{
		public static float2 LerpTo(this float2 a, float2 b, float t)
		{
			return a + (b - a) * t;
		}
	}
}
