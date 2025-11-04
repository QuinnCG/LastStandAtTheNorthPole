using UnityEngine.VFX;

namespace Quinn
{
	public static class VisualEffectExtensions
	{
		/// <summary>
		/// Checks if the VFX has any alive particles.
		/// </summary>
		public static bool IsPlaying(this VisualEffect vfx)
		{
			return vfx.aliveParticleCount > 0;
		}
	}
}
