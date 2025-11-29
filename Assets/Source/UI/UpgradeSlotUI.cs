using DG.Tweening;
using FMODUnity;
using Quinn.PlayerSystem;
using Quinn.PlayerSystem.Upgrades;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class UpgradeSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
	{
		[SerializeField, Required]
		private Image Icon;
		[SerializeField, Required]
		private TextMeshProUGUI Details;
		[SerializeField, Required]
		private TextMeshProUGUI Multiplier;
		[SerializeField]
		private EventReference HoverSound, ClickSound;

		public UpgradeSO? Upgrade { get; private set; }
		public event System.Action<UpgradeSO>? OnUpgradeSelected;

		private bool _disabled;

		public void Init(UpgradeSO upgrade, float multiplier = 1f, bool disable = false)
		{
			Upgrade = upgrade;

			Multiplier.text = $"{multiplier:0.00}x";
			Multiplier.enabled = upgrade.Upgrade is WeaponUpgrade;

			transform.DOKill();
			transform.localScale = Vector3.one;

			Icon.sprite = upgrade.Icon;
			Details.text = upgrade.Details;

			_disabled = disable;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (_disabled)
				return;

			if (!UpgradeUI.Instance.CanUpgradeBeSelected)
				return;

			Audio.Play(HoverSound);
			transform.DOLocalScale(Vector3.one * 1.1f, 0.1f);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (_disabled)
				return;

			if (!UpgradeUI.Instance.CanUpgradeBeSelected)
				return;

			transform.DOLocalScale(Vector3.one * 1f, 0.1f);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (_disabled)
				return;

			if (!UpgradeUI.Instance.CanUpgradeBeSelected)
				return;

			transform.DOLocalScale(Vector3.one * 0.9f, 0.1f);
			UpgradeUI.Instance.Hide();

			Audio.Play(ClickSound);
			OnUpgradeSelected?.Invoke(Upgrade!);
		}
	}
}
