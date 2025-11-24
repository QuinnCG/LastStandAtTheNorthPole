using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections;
using System.Text;
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

		private void Awake()
		{
			Instance = this;
		}

		public async Awaitable PlayNewWaveSequenceAsync()
        {
            StopAllCoroutines();

            var rect = WaveCount.GetComponent<RectTransform>();

            var pos = rect.anchoredPosition;
            var scale = rect.localScale;

            float inDur = 1f;

            rect.DOAnchorPos(WaveCountTarget.anchoredPosition, inDur).SetEase(Ease.InCubic);
            rect.DOLocalScale(WaveCountTarget.localScale, inDur).SetEase(Ease.OutBack);

            await Wait.Duration(inDur);
            SetRandomRotationForWaveCount(rect);
            StartCoroutine(TypeWave());

            await Wait.Duration(3f);

            float outDur = 0.2f;

            rect.DOAnchorPos(pos, outDur).SetEase(Ease.Linear);
            rect.DORotateZ(0f, outDur).SetEase(Ease.Linear);
            rect.DOLocalScale(scale, outDur).SetEase(Ease.Linear);

            WaveCount.text = WaveManager.Instance.WaveNumber.ToString();

            // TODO: Play punch SFX at apex of anim.
            // TODO: Play VFX.
            // TODO: Add horizontal screen stretched ribbon.
        }

        private IEnumerator TypeWave()
		{
			int waveNum = WaveManager.Instance.WaveNumber;
			string text = $"Wave\n{waveNum}";

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
					yield return new WaitForSeconds(WriteInterval * i);
				}
			}
		}

		private void SetRandomRotationForWaveCount(RectTransform rect)
		{
			rect.rotation = Quaternion.AngleAxis(Random.Range(-WaveCountRotAngle, WaveCountRotAngle), Vector3.forward);
		}
	}
}
