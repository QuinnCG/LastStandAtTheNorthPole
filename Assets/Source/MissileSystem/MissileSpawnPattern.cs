using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.MissileSystem
{
	[System.Serializable]
	public struct MissileSpawnPattern
	{
		[MinValue(1)]
		public int Count;

		[Space]

		public bool IsRandom;
		public MissileSpreadType SpreadType;
		[MinValue(0)]
		[Unit(Units.Degree), ShowIf(nameof(SpreadType), MissileSpreadType.Arc)]
		public float ArcSpread;
	}
}
