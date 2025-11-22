using DG.Tweening;
using FMODUnity;
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
		[SerializeField]
		private EventReference HoverSound, ClickSound;

		public UpgradeSO? Upgrade { get; private set; }

		public event System.Action<UpgradeSO>? OnUpgradeSelected;

		public void Init(UpgradeSO upgrade)
		{
			Upgrade = upgrade;

			transform.DOKill();
			transform.localScale = Vector3.one;

			Icon.sprite = upgrade.Icon;
			Details.text = upgrade.Details;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!UpgradeUI.Instance.CanUpgradeBeSelected)
				return;

			Audio.Play(HoverSound);
			transform.DOLocalScale(Vector3.one * 1.1f, 0.1f);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!UpgradeUI.Instance.CanUpgradeBeSelected)
				return;

			transform.DOLocalScale(Vector3.one * 1f, 0.1f);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!UpgradeUI.Instance.CanUpgradeBeSelected)
				return;

			transform.DOLocalScale(Vector3.one * 0.9f, 0.1f);
			UpgradeUI.Instance.Hide();

			Audio.Play(ClickSound);

			OnUpgradeSelected?.Invoke(Upgrade!);
		}
	}
}
