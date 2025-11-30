using Quinn.UI;
using Quinn.WaveSystem;
using Sirenix.OdinInspector;
using System;
using System.Threading.Tasks;
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

		private void Awake()
		{
			Instance = this;
			InUpgradeSequence = true;

			SceneManager.sceneLoaded += OnSceneLoaded;
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
				u1 = StartingUpgrades[0];
				u2 = StartingUpgrades[1];
				u3 = StartingUpgrades[2];
			}
			else
			{
				u1 = AllUpgrades.GetRandom();
				u2 = AllUpgrades.GetRandom();
				u3 = AllUpgrades.GetRandom();
			}

			_upgradeSelected = false;
			UpgradeUI.Instance.Show(u1, u2, u3);

			await Wait.Until(() => _upgradeSelected);
			_upgradeSelected = false;

			InUpgradeSequence = false;
		}
	}
}
