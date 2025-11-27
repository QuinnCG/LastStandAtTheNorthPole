using QFSW.QC;
using Quinn.AI;
using Quinn.DamageSystem;
using Quinn.PlayerSystem;
using Quinn.UI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

namespace Quinn.WaveSystem
{
	public class WaveManager : MonoBehaviour
	{
		[System.Serializable]
		public record WaveDefinition
		{
			public int MinimumWaveNumber = 1;
			[InlineProperty, HideLabel]
			public WaveProfile Profile;
		}

		public static WaveManager Instance { get; private set; }

		[SerializeField]
		private float WaveDifficultyDelta = 0.1f;
		[SerializeField]
		private float PlayerNoSpawnRadius = 10f;
		[SerializeField, InlineProperty]
		private WaveDefinition[] Waves;

		public int WaveNumber { get; private set; } = 0;
		public float WaveDifficultyFactor { get; set; } = 1f;
		public int AliveInCurrentWave { get; private set; }
		public Vector2 LastEnemyPos
		{
			get
			{
				if (AliveInCurrentWave > 0)
				{
					return _aliveEnemies.First().bounds.center;
				}

				return Vector2.zero;
			}
		}

		private readonly HashSet<Collider2D> _aliveEnemies = new();
		private bool _forceStartNextSubWave;

		private void Awake()
		{
			Instance = this;
		}

        private void Start()
        {
			StartNextWave();
        }

        [Command("wave.next")]
		public async void StartNextWave()
		{
			await UpgradeManager.Instance.BeginUpgradeSequenceAsync();

			WaveDifficultyFactor += WaveDifficultyDelta;
			WaveNumber++;

			await WaveManagerUI.Instance.PlayNewWaveSequenceAsync();
			StartCoroutine(WaveSequence());
		}

		[Command("wave.reset")]
		public void ResetWave()
		{
			WaveNumber = 0;
			WaveDifficultyFactor = 1f;
		}

		[Command("wave.difficulty")]
		protected void GetWaveDifficulty()
		{
			Log.Info($"Current wave difficulty: {WaveDifficultyFactor}");
		}

		private IEnumerator WaveSequence()
		{
			var profile = GetRandomValidWave();

			for (int i = 0; i < profile.SubWaveCount.GetRandom(); i++)
			{
				foreach (var entry in profile.SpawnDefinitions)
				{
					SpawnBatch(entry.Prefab!, entry.GetRandomSpawnCount(WaveDifficultyFactor));
				}

				float endTime = Time.time + profile.SpawnInterval.GetRandom();
				yield return new WaitUntil(() => Time.time >= endTime || _forceStartNextSubWave);
				_forceStartNextSubWave = false;
			}

			yield return new WaitUntil(() => AliveInCurrentWave <= 0);
			StartNextWave();
		}

		private void SpawnBatch(GameObject prefab, int count)
		{
			for (int i = 0; i < count; i++)
			{
				var dir = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * Vector2.up;
				var pos = Player.Instance.transform.position + (dir * PlayerNoSpawnRadius);

				AliveInCurrentWave++;

				var instance = prefab.Clone(pos);
				var hitbox = instance.GetComponent<Collider2D>();
				_aliveEnemies.Add(hitbox);

				instance.GetComponent<Health>().OnDeath += _ =>
				{
					AliveInCurrentWave--;
					_aliveEnemies.Remove(hitbox);

					if (AliveInCurrentWave <= 0)
					{
						_forceStartNextSubWave = true;
					}
				};
			}
		}

		private WaveProfile GetRandomValidWave()
		{
			var filtered = Waves.Where(x => WaveNumber >= x.MinimumWaveNumber);
			return filtered.GetRandom().Profile;
		}
	}
}
