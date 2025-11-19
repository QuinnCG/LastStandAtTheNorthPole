namespace Quinn.DamageSystem
{
	/// <summary>
	/// Created automatically after damage is applied. This is what's passed through on damage events and similar.
	/// </summary>
	public record DamageInstance
	{
		public DamageInfo? Info;

		/// <summary>
		/// The final damage that was attempted to be applied, after accounting for resistances/weaknesses.
		/// </summary>
		public float FinalDamage;
		/// <summary>
		/// The actual damage that was applied, after accounting for remaining health.
		/// </summary>
		public float RealDamage;
		public bool WasLethal;

		public bool WasResisted;
		public bool WasWeak;
	}
}
