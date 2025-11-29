using DG.Tweening;
using FMODUnity;
using Quinn.DamageSystem;
using Quinn.PlayerSystem;
using Quinn.WaveSystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Quinn.UI
{
	public class WaveManagerUI : MonoBehaviour
	{
		[SerializeField, Required]
		private TextMeshProUGUI WaveCount;
		[SerializeField, Required]
		private RectTransform WaveCountTarget;
		[SerializeField, Unit(Units.Second)]
		private float WriteInterval = 0.1f;
		[SerializeField]
		private EventReference WritesSound;
		[SerializeField, Unit(Units.Degree)]
		private float WaveCountRotAngle = 15f;

		public static WaveManagerUI Instance { get; private set; }

		private Vector2 _defaultPos;
		private Vector3 _defaultScale;

		private void Awake()
		{
			Instance = this;

			var rect = WaveCount.GetComponent<RectTransform>();

			_defaultPos = rect.anchoredPosition;
			_defaultScale = rect.localScale;
		}

        private void Start()
        {
			Player.Instance.GetComponent<Health>().OnDeath += OnDeath;
        }

        private async void OnDeath(DamageInstance dmgInstance)
        {
			TweenIn(0.5f);
			await Wait.Duration(0.5f);

			int waveNumber = WaveManager.Instance.WaveNumber;
			string wave = waveNumber > 1 ? "Waves" : "Wave";
			StartCoroutine(Type($"Survivded {waveNumber} {wave}!", 0.01f));
        }

        public async Awaitable PlayNewWaveSequenceAsync()
        {
            StopAllCoroutines();

			var rect = WaveCount.GetComponent<RectTransform>();

			const float inDur = 1f;
			TweenIn(inDur);

            await Wait.Duration(inDur);
            SetRandomRotationForWaveCount(rect);

			int waveNum = WaveManager.Instance.WaveNumber;
			StartCoroutine(Type($"Wave\n{waveNum}", WriteInterval));

            await Wait.Duration(3f);

			TweenOut();
            WaveCount.text = WaveManager.Instance.WaveNumber.ToString();

            // TODO: Play punch SFX at apex of anim.
            // TODO: Play VFX.
            // TODO: Add horizontal screen stretched ribbon.
        }

		private void TweenIn(float inDur)
		{
			var rect = WaveCount.GetComponent<RectTransform>();

			rect.DOAnchorPos(WaveCountTarget.anchoredPosition, inDur).SetEase(Ease.InCubic);
			rect.DOLocalScale(WaveCountTarget.localScale, inDur).SetEase(Ease.OutBack);
		}

		private void TweenOut()
		{
			const float outDur = 0.2f;
			var rect = WaveCount.GetComponent<RectTransform>();

			rect.DOAnchorPos(_defaultPos, outDur).SetEase(Ease.Linear);
			rect.DORotateZ(0f, outDur).SetEase(Ease.Linear);
			rect.DOLocalScale(_defaultScale, outDur).SetEase(Ease.Linear);
		}

        private IEnumerator Type(string text, float interval)
		{
			var builder = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
			{
                char c = text[i];
                builder.Append(c);
				WaveCount.text = builder.ToString();

				Audio.Play(WritesSound);

				if (i == text.Length - 1)
				{
					SetRandomRotationForWaveCount(WaveCount.GetComponent<RectTransform>());
				}
				else
				{
					yield return new WaitForSeconds(interval * i);
				}
			}
		}

		private void SetRandomRotationForWaveCount(RectTransform rect)
		{
			rect.rotation = Quaternion.AngleAxis(Random.Range(-WaveCountRotAngle, WaveCountRotAngle), Vector3.forward);
		}
	}
}
