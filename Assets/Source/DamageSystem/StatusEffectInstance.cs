using Sirenix.OdinInspector;

namespace Quinn.DamageSystem
{
	[System.Serializable, InlineProperty]
	public record StatusEffectInstance
	{
		[HideLabel, HorizontalGroup]
		public StatusEffect Status;
		[HideLabel, HorizontalGroup, Unit(Units.Second)]
		public float Duration = 5f;
	}
}
