using DG.Tweening;
using Quinn.DamageSystem;
using Quinn.PlayerSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class VignetteUI : MonoBehaviour
	{
		[SerializeField]
		private float HurtDuration = 1.5f;
		[SerializeField]
		private float FadeInDuration = 0.2f;
		[SerializeField]
		private float FadeOutDuration = 1f;

		[SerializeField, Required]
		private Image HurtVignette;

		public void Start()
		{
			Player.Instance.GetComponent<Health>().OnDamage += OnHurt;
		}

		private void OnHurt(DamageInstance dmg)
		{
			var seq = DOTween.Sequence()
				.SetTarget(HurtVignette);

			seq.Append(HurtVignette.DOFade(1f, FadeInDuration));
			seq.AppendInterval(HurtDuration);
			seq.Append(HurtVignette.DOFade(0f, FadeOutDuration));
		}
	}
}
