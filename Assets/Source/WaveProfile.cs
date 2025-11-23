using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
    [CreateAssetMenu]
	public class WaveProfile : ScriptableObject
	{
		public Vector2 SpawnInterval = new(3f, 7f);
		[InlineProperty]
		public EnemySpawnDefinition[] SpawnDefinitions = System.Array.Empty<EnemySpawnDefinition>();
	}
}
