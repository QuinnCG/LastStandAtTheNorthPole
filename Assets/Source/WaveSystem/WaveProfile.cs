using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.WaveSystem
{
	[CreateAssetMenu]
	public class WaveProfile : ScriptableObject
	{
		public Vector2 SpawnInterval = new(3f, 7f);
		public Vector2Int SubWaveCount = new(1, 3);
		[InlineProperty]
		public EnemySpawnDefinition[] SpawnDefinitions = System.Array.Empty<EnemySpawnDefinition>();
	}
}
