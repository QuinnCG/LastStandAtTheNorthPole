using DG.Tweening;
using FMODUnity;
using Quinn.DamageSystem;
using Quinn.PlayerSystem;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class HeartsUI : MonoBehaviour
	{
		[SerializeField, Required]
		private GameObject HeartPrefab;
		[SerializeField, Required]
		private Sprite FullHeart, HalfHeart, EmptyHeart;

		[SerializeField, BoxGroup("Last Heart Shake")]
		private float LastHeartAmplitude = 5f, LastHeartFrequency = 50f;

		[SerializeField, BoxGroup("HP Change Punch")]
		private float PunchScale = 1.1f, PunchDur = 0.2f;
		[SerializeField, BoxGroup("HP Change Punch")]
		private Ease PunchBigEase = Ease.Linear, PunchSmallEase = Ease.Linear;

		[SerializeField, BoxGroup("SFX")]
		private EventReference GainHeartSound;

		public static HeartsUI Instance { get; private set; }

		private readonly List<Image> _hearts = new();
		private Health _health;

		public void Awake()
		{
			Instance = this;
		}

		public void Start()
		{
			_health = Player.Instance.GetComponent<Health>();

			_health.OnDamage += info =>
			{
				OnHealthChange(-info.RealDamage);
			};
			_health.OnHeal += hp =>
			{
				OnHealthChange(hp);
			};

			transform.DestroyChildren();
			ReconstructHearts();
		}

		public void Update()
		{
			if (_health != null && _health.Current <= 1f)
			{
				transform.localPosition = new Vector3()
				{
					x = Mathf.Sin(Time.time * LastHeartFrequency),
					y = Mathf.Cos((Time.time + Random.value) * LastHeartFrequency)
				} * LastHeartAmplitude;
			}
			else
			{
				transform.localPosition = Vector3.zero;
			}
		}

		private async void OnHealthChange(float delta)
		{
			if (delta != 0f)
			{
				if (delta > 0f)
				{
					Audio.Play(GainHeartSound);
				}

				UpdateHearts(isHealing: delta > 0f);

				await transform.DOScale(PunchScale, PunchDur / 2f)
					.SetEase(PunchBigEase)
					.AsyncWaitForCompletion();

				transform.DOScale(1, PunchDur / 2f)
					.SetEase(PunchSmallEase);
			}
		}

		private void ReconstructHearts()
		{
			foreach (var heart in _hearts.ToArray())
			{
				Destroy(heart.gameObject);
			}

			_hearts.Clear();

			int max = Mathf.RoundToInt(_health.Max);

			for (int i = 0; i < max; i++)
			{
				var heart = HeartPrefab.Clone(transform);
				_hearts.Add(heart.GetComponent<Image>());
			}
		}

		private void UpdateHearts(bool isHealing = false)
		{
			int current = Mathf.CeilToInt(_health.Current);

			for (int i = 0; i < _hearts.Count; i++)
			{
				var child = _hearts[i];

				bool isNotEmpty = i < current;
				child.sprite = isNotEmpty ? FullHeart : EmptyHeart;

				if (isNotEmpty && _health.IsHalfHeart)
				{
					child.sprite = HalfHeart;
				}

				if (isNotEmpty && isHealing)
				{
					PunchHeart(child.transform, i);
				}
			}
		}

		private async void PunchHeart(Transform heart, int index)
		{
			await Awaitable.WaitForSecondsAsync(index * 0.1f);

			await heart.DOScale(2f, 0.1f).AsyncWaitForCompletion();
			heart.DOScale(1f, 0.1f);
		}
	}
}
