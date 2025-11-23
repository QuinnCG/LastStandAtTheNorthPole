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

		public async void PlayNewWaveSequence()
		{
			StopAllCoroutines();

			var pos = WaveCount.transform.localPosition;
			var scale = WaveCount.transform.localScale;

			var rect = WaveCount.GetComponent<RectTransform>();

			float duration = 0.5f;

			rect.DOAnchorPos(WaveCountTarget.anchoredPosition, duration);
			rect.DOLocalScale(WaveCountTarget.localScale, duration);

			await Wait.Duration(duration);
			rect.rotation = Quaternion.AngleAxis(Random.Range(-WaveCountRotAngle, WaveCountRotAngle), Vector3.forward);
			StartCoroutine(TypeWave());

			// TODO: Wait, then return to position faster.
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
				yield return new WaitForSeconds(WriteInterval * i);
			}
		}
	}
}
