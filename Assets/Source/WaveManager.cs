using QFSW.QC;
using Quinn.PlayerSystem;
using Quinn.UI;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Quinn
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
		[SerializeField, InlineProperty]
		private WaveDefinition[] Waves;

		public int WaveNumber { get; private set; } = 0;
		public float WaveDifficultyFactor { get; set; } = 1f;

		// When to spawn in the next enemy.
		private float _nextSpawnTime;

		private void Awake()
		{
			Instance = this;
		}

		[Command("wave.next")]
		public async void StartNextWave()
		{
			await UpgradeManager.Instance.BeginUpgradeSequenceAsync();

			WaveDifficultyFactor += WaveDifficultyDelta;
			WaveNumber++;

			await WaveManagerUI.Instance.PlayNewWaveSequenceAsync();

			StartCoroutine(BeginSpawningWave());
		}

		[Command("wave.difficulty")]
		protected void GetWaveDifficulty()
		{
			Log.Info($"Current wave difficulty: {WaveDifficultyFactor}");
		}

		private IEnumerator BeginSpawningWave()
		{
			yield break;
			// TODO: Implement.
		}
	}
}
