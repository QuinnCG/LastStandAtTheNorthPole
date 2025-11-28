using DG.Tweening;
using QFSW.QC;
using Quinn.DamageSystem;
using Quinn.PlayerSystem;
using Quinn.WaveSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.UI
{
	public class UpgradeUI : MonoBehaviour
	{
		public static UpgradeUI Instance { get; private set; }

		[SerializeField, Required]
		private CanvasGroup CanvasGroup;
		[SerializeField, Required]
		private UpgradeSlotUI[] UpgradeSlots;
		[SerializeField, Required]
		private UpgradeSlotUI EquippedGunSlot;
		[SerializeField, Required]
		private CanvasGroup EquippedGunSlotCanvasGroup;
		[SerializeField]
		private float FadeInTime = 0.1f, FadeOutTime = 0.1f;

		public event System.Action<UpgradeSO>? OnUpgradeSelected;

		public bool CanUpgradeBeSelected { get; private set; }

		private void Awake()
		{
			Instance = this;
			CanvasGroup.alpha = 0f;

			foreach (var slot in UpgradeSlots)
			{
				slot.OnUpgradeSelected += (upgrade) =>
				{
					OnUpgradeSelected?.Invoke(upgrade);
				};
			}
		}

		private void OnDestroy()
		{
			if (InputManager.Instance != null)
			{
				InputManager.Instance.UnblockInput(this);
			}
		}

		public void Show(UpgradeSO upgrade1, UpgradeSO upgrade2, UpgradeSO upgrade3)
		{
			var equipped = GunManager.Instance.Equipped;
			if (equipped != null)
			{
				EquippedGunSlot.Init(GunManager.Instance.EquippedGunUpgradeSO!, GunManager.Instance.EquippedMultiplier, disable: true);
				EquippedGunSlotCanvasGroup.alpha = 1f;
			}
			else
			{
				EquippedGunSlotCanvasGroup.alpha = 0f;
			}

			CanvasGroup.DOFade(1f, FadeInTime);
			CanUpgradeBeSelected = true;

			float multiplier = GunManager.Instance.CurrentWaveStatMultiplier;

			UpgradeSlots[0].Init(upgrade1, multiplier);
			UpgradeSlots[1].Init(upgrade2, multiplier);
			UpgradeSlots[2].Init(upgrade3, multiplier);

			InputManager.Instance.RegisterShowCursor(this);
			InputManager.Instance.BlockInput(this);
			CrosshairManager.Instance.HideCrosshair();
			Player.Instance.GetComponent<Health>().BlockDamage(this);
			TransitionManager.Instance.MuffleAudio();
		}

		[Command("upgrade_menu.hide")]
		public void Hide()
		{
			CanUpgradeBeSelected = false;
			CanvasGroup.DOFade(0f, FadeOutTime);

			InputManager.Instance.UnregisterShowCursor(this);
			InputManager.Instance.UnblockInput(this);

			CrosshairManager.Instance.ShowCrosshair();
			Player.Instance.GetComponent<Health>().UnblockDamage(this);
			TransitionManager.Instance.UnmuffleAudio();
		}

		[Command("upgrade_menu.show")]
		protected void ForceShowUpgradeMenu_Cmd()
		{
			CanvasGroup.DOFade(1f, FadeInTime);

			InputManager.Instance.RegisterShowCursor(this);
			InputManager.Instance.BlockInput(this);
			CanUpgradeBeSelected = true;
			CrosshairManager.Instance.HideCrosshair();
			Player.Instance.GetComponent<Health>().BlockDamage(this);
			TransitionManager.Instance.MuffleAudio();
		}
	}
}
