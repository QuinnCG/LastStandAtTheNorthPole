using Quinn.UI;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	public class UpgradeManager : MonoBehaviour
	{
		public static UpgradeManager Instance { get; private set; }

		[SerializeField]
		private UpgradeSO[] AllUpgrades;

		private void Awake()
		{
			Instance = this;
		}

		public void BeginUpgradeSequence()
		{
			UpgradeUI.Instance.Show(AllUpgrades.GetRandom(), AllUpgrades.GetRandom(), AllUpgrades.GetRandom());
		}
	}
}
