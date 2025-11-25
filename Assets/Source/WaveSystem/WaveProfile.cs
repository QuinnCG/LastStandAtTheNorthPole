using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.WaveSystem
{
	[CreateAssetMenu]
	public class WaveProfile : ScriptableObject
	{
		public Vector2 SpawnInterval = new(20f, 30f);
		public Vector2Int SubWaveCount = new(2, 3);
		[InlineProperty]
		public EnemySpawnDefinition[] SpawnDefinitions = System.Array.Empty<EnemySpawnDefinition>();
	}
}
