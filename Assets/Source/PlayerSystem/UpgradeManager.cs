using Quinn.UI;
using Quinn.WaveSystem;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Quinn.PlayerSystem
{
	public class UpgradeManager : MonoBehaviour
	{
		public static UpgradeManager Instance { get; private set; }

		[SerializeField, RequiredListLength(3, 3)]
		private UpgradeSO[] StartingUpgrades;
		[SerializeField]
		private UpgradeSO[] AllUpgrades;

		public bool InUpgradeSequence { get; private set; }

		private bool _upgradeSelected;
		private readonly Dictionary<UpgradeSO, int> _appearanceCount = new();

		private readonly HashSet<UpgradeSO> _availableUpgrades = new();

		private void Awake()
		{
			Instance = this;
			InUpgradeSequence = true;

			SceneManager.sceneLoaded += OnSceneLoaded;
			_availableUpgrades.AddRange(AllUpgrades);
		}

        private void OnDestroy()
        {
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

        private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
			await Wait.Until(() => UpgradeUI.Instance != null);
			UpgradeUI.Instance.OnUpgradeSelected += OnUpgradeSelected;
		}

        private void OnUpgradeSelected(UpgradeSO upgrade)
		{
			_upgradeSelected = true;
			upgrade.Upgrade.ApplyUpgrade();
		}

		public async Awaitable BeginUpgradeSequenceAsync()
		{
			InUpgradeSequence = true;

			UpgradeSO u1, u2, u3;

			// First wave.
			if (WaveManager.Instance.WaveNumber <= 1)
			{
				var indices = new HashSet<int>() { 0, 1, 2 };

				// HACK

				int index = indices.GetRandom();
				u1 = StartingUpgrades[index];
				indices.Remove(index);

				index = indices.GetRandom();
				u2 = StartingUpgrades[index];
				indices.Remove(index);

				index = indices.GetRandom();
				u3 = StartingUpgrades[index];
				indices.Remove(index);
			}
			else
			{
				u1 = GetRandomUpgrade();
				u2 = GetRandomUpgrade();
				u3 = GetRandomUpgrade();
			}

			_upgradeSelected = false;
			UpgradeUI.Instance.Show(u1, u2, u3);

			await Wait.Until(() => _upgradeSelected);
			_upgradeSelected = false;

			InUpgradeSequence = false;
		}

		private UpgradeSO GetRandomUpgrade()
		{
			int attempts = 0;

			while (true)
			{
				var upgrade = _availableUpgrades.GetRandom();

				if (attempts > 500)
				{
					Log.Warning("Failed after 500 attempts! Using fallback upgrade!");
					return upgrade;
				}

				if (_appearanceCount.TryGetValue(upgrade, out int count))
				{
					if (count > upgrade.MaxAppearanceCount)
					{
						attempts++;
						_appearanceCount.Remove(upgrade);
						continue;
					}
					else
					{
						_appearanceCount[upgrade] = count + 1;
						return upgrade;
					}
				}
				else
				{
					_appearanceCount.Add(upgrade, 1);
					return upgrade;
				}
			}
		}
	}
}
