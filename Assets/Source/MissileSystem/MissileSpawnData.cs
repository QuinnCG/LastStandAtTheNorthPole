using Sirenix.OdinInspector;

namespace Quinn.MissileSystem
{
	/// <summary>
	/// A utility class that aggregates the missile to the missile spawn pattern.
	/// </summary>
	[InlineProperty, HideLabel, FoldoutGroup("Missile Spawn Data")]
	[System.Serializable]
	public record MissileSpawnData
	{
		[HideLabel, InlineEditor]
		public MissileData Missile;
		[HideLabel]
		public MissileSpawnPattern Pattern;
	}
}
