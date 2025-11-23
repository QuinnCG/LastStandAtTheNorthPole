using UnityEngine;

namespace Quinn
{
    [System.Serializable]
	public record EnemySpawnDefinition
	{
		public GameObject? Prefab;
		public Vector2Int BaseSpawnCount = new(1, 3);

		public int GetRandomSpawnCount(float difficultyFactor)
		{
			return Mathf.RoundToInt(BaseSpawnCount.GetRandom() * difficultyFactor);
		}
	}
}
