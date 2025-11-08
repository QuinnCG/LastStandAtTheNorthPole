using UnityEngine;

namespace Quinn.DamageSystem
{
	/// <summary>
	/// Used when applying damage.
	/// </summary>
	public record DamageInfo
	{
		public float Damage = 1f;
		public Team Team = Team.Environment;
		public Vector2 Direction;
		public float Knockback;
		public StatusEffectInstance[] StatusEffects = System.Array.Empty<StatusEffectInstance>();
	}
}
