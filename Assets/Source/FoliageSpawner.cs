using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class FoliageSpawner : MonoBehaviour
	{
		[SerializeField, Required]
		private GameObject Prefab;
		[SerializeField, Range(0f, 1f)]
		private float SpawnChance = 0.5f;
		[SerializeField]
		private float NudgeOffset = 0.2f;
		[SerializeField]
		private int MaxBatchIterationCount = 100;

		private IEnumerator Start()
		{
			var bounds = GetComponent<BoxCollider2D>().bounds;

			int iteration = 0;

			for (int x = Mathf.RoundToInt(bounds.Left().x); x <= Mathf.RoundToInt(bounds.Right().x); x++)
			{
				for (int y = Mathf.RoundToInt(bounds.Bottom().y); y <= Mathf.RoundToInt(bounds.Top().y); y++)
				{
					if (Random.value < SpawnChance)
					{
						var pos = new Vector2(x, y);
						pos += Random.insideUnitCircle * NudgeOffset;
						Prefab.Clone(pos, transform);
					}

					iteration++;

					if (iteration > MaxBatchIterationCount)
					{
						// Wait untill next frame.
						iteration = 0;
						yield return null;
					}
				}
			}
		}
	}
}
