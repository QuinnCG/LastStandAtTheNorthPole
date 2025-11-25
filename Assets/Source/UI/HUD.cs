using DG.Tweening;
using FMODUnity;
using Quinn.DamageSystem;
using Quinn.PlayerSystem;
using Quinn.WaveSystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class HUD : MonoBehaviour
	{
		[SerializeField, Required]
		private CanvasGroup Group;
		[SerializeField]
		private float FadeDuration = 1f;
		[SerializeField, Required]
		private TextMeshProUGUI AliveCount;

		[SerializeField, Required, Space]
		private Image LowHPVignette;
		[SerializeField]
		private float SmoothTime = 0.2f;
		[SerializeField]
		private float ScaleFrequency = 0.5f;
		[SerializeField]
		private float ScaleAmplitude = 0.2f;

		[SerializeField, Required, Space]
		private TextMeshProUGUI Dialogue;
		[SerializeField]
		private float WriteInterval = 0.01f;
		[SerializeField]
		private EventReference WriteSound;

		[Space]

		[SerializeField]
		private string StartDialogue = "Somewhere in the North Pole...";
		[SerializeField]
		private float StartDialogueDuration = 5f;

		public static HUD Instance { get; private set; }

		public float Alpha => Group.alpha;

		private CancellationTokenSource _cancelDialogue = new();

		private float _alphaVel;
		private float _defaultScale;

		private Health? _playerHealth;

		public void Awake()
		{
			Instance = this;

			Dialogue.alpha = 0f;
			_defaultScale = LowHPVignette.transform.localScale.y;
		}

		private IEnumerator Start()
		{
			_playerHealth = Player.Instance.GetComponent<Health>();

			WriteDialogue(StartDialogue);
			yield return new WaitForSeconds(StartDialogueDuration);
			HideDialogue();
		}

		public void Update()
		{
			var color = LowHPVignette.color;

			float targetAlpha;
			if (_playerHealth!.IsDead || _playerHealth.Current <= 1.55f) // HACK
			{
				targetAlpha = 1f;
			}
			else if (_playerHealth.Current < 2.55f) // HACK
			{
				targetAlpha = 0.3f;
			}
			else
			{
				targetAlpha = 0f;
			}

			color.a = Mathf.SmoothDamp(LowHPVignette.color.a, targetAlpha, ref _alphaVel, SmoothTime);
			LowHPVignette.color = color;

			float scale = Mathf.Sin(Time.time * ScaleFrequency) * ScaleAmplitude;
			scale += _defaultScale;
			LowHPVignette.transform.localScale = new Vector3(scale, scale, 1f);
		}

        private void LateUpdate()
        {
            AliveCount.text = $"Alive: {WaveManager.Instance.AliveInCurrentWave}";
		}

        public void OnDestroy()
		{
			Group.DOKill();
			_cancelDialogue.Cancel();
		}

		public void Hide()
		{
			Group.alpha = 0f;
		}

		public void FadeIn()
		{
			Group.DOFade(1f, FadeDuration);
		}

		public async void WriteDialogue(string text)
		{
			_cancelDialogue.Cancel();
			_cancelDialogue = new();

			Dialogue.text = string.Empty;

			if (Dialogue.alpha == 0f)
			{
				Dialogue.DOFade(1f, 0.1f);
			}

			var builder = new StringBuilder();

			for (int i = 0; i < text.Length; i++)
			{
				if (_cancelDialogue.IsCancellationRequested)
				{
					return;
				}

				builder.Append(text[i]);
				Dialogue.text = builder.ToString();

				Audio.Play(WriteSound);
				await Awaitable.WaitForSecondsAsync(WriteInterval, _cancelDialogue.Token);
			}
		}

		public void HideDialogue()
		{
			Dialogue.DOFade(0f, 0.1f);
		}
	}
}
