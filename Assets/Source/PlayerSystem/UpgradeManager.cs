using Quinn.UI;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	public class UpgradeManager : MonoBehaviour
	{
		public static UpgradeManager Instance { get; private set; }

		[SerializeField]
		private UpgradeSO[] AllUpgrades;

		private bool _upgradeSelected;

		private void Awake()
		{
			Instance = this;
		}

        private void Start()
        {
			UpgradeUI.Instance.OnUpgradeSelected += OnUpgradeSelected;
		}

        private void OnUpgradeSelected(UpgradeSO upgrade)
        {
			_upgradeSelected = true;
        }

		// TODO: Apply new upgrade.

        public async Awaitable BeginUpgradeSequenceAsync()
		{
			_upgradeSelected = false;
			UpgradeUI.Instance.Show(AllUpgrades.GetRandom(), AllUpgrades.GetRandom(), AllUpgrades.GetRandom());

			await Wait.Until(() => _upgradeSelected);
		}
	}
}
