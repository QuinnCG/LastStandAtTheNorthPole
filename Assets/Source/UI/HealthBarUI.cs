using DG.Tweening;
using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.UI
{
	[RequireComponent(typeof(Health))]
	public class HealthBarUI : MonoBehaviour
	{
		[SerializeField, Required]
		private Slider HPBar;
		[SerializeField, Required]
		private CanvasGroup CanvasGroup;

		[SerializeField]
		private float FadeInTime = 0.1f, FadeOutTime = 0.5f;
		[SerializeField]
		private float PunchScale = 1.1f, PunchTime = 0.1f;

		private Health _health;

		private void Awake()
		{
			CanvasGroup.alpha = 0f;
			_health = GetComponent<Health>();

			_health.OnDamaged += _ =>
			{
				HPBar.transform.DOScale(Vector3.one * PunchScale, PunchTime)
					.SetLoops(2, LoopType.Yoyo);

				if (_health.Normalized <= 0f)
				{
					CanvasGroup.DOFade(0f, FadeOutTime);
				}
				else
				{
					CanvasGroup.DOFade(1f, FadeInTime);
				}
			};

			_health.OnHeal += _ =>
			{
				if (_health.Normalized >= 1f)
				{
					CanvasGroup.DOFade(0f, FadeOutTime);
				}
			};
		}

		private void OnDestroy()
		{
			if (HPBar != null)
			{
				HPBar.transform.DOKill();
				CanvasGroup.DOKill();
			}
		}

		private void LateUpdate()
		{
			HPBar.value = _health.Normalized;
			HPBar.gameObject.SetActive(_health.IsAlive);
		}
	}
}
