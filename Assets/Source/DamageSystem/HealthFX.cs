using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn.DamageSystem
{
	[RequireComponent(typeof(Health))]
	public class HealthFX : MonoBehaviour
	{
		public const string FlashName = "_Flash";

		[SerializeField]
		private bool DoesFlash;
		[SerializeField]
		private float FlashHold = 0.1f, FlashOut = 0.1f;

		[SerializeField, InfoBox("@BlinkCount * BlinkVisible * BlinkInvisible")]
		private bool DoesBlink;
		[SerializeField]
		private int BlinkCount = 6;
		[SerializeField, Unit(Units.Second)]
		private float BlinkVisible = 0.1f, BlinkInvisible = 0.1f;

		[SerializeField]
		private EventReference RegularHurtSFX, WeakHurtSFX, ResistantHurtSFX, DeathSFX;

		[SerializeField, ChildGameObjectsOnly]
		private VisualEffect HurtVFX, DeathVFX;
		[SerializeField]
		private StringToggle DirectionFieldVFX = new() { Value = "Direction" };

		[Space, SerializeField]
		private SpriteRenderer[] Renderers;

		private void OnValidate()
		{
			if (DoesFlash && Renderers != null)
			{
				foreach (var renderer in Renderers)
				{
					if (!renderer.sharedMaterial.HasFloat("_Flash"))
					{
						Log.Error($"Renderer '{renderer.gameObject.name}' doesn't have the required property '{FlashName}'!");
					}
				}
			}
		}

		private void Awake()
		{
			var health = GetComponent<Health>();
			health.OnDamage += OnDamage;
			health.OnDeath += OnDeath;
		}

		private void OnDamage(DamageInstance dmgInstance)
		{
			if (dmgInstance.WasResisted)
			{
				Audio.Play(ResistantHurtSFX, transform.position);
			}
			else if (dmgInstance.WasResisted)
			{
				Audio.Play(WeakHurtSFX, transform.position);
			}
			else
			{
				Audio.Play(RegularHurtSFX, transform.position);
			}

			if (HurtVFX != null)
			{
				if (DirectionFieldVFX.IsEnabled)
				{
					HurtVFX.SetVector2(DirectionFieldVFX.Value, dmgInstance.Info.Direction);
				}

				HurtVFX.Play();
			}

			if (DoesFlash)
			{
				foreach (var renderer in Renderers)
				{
					renderer.material.SetFloat(FlashName, 1f);

					var seq = DOTween.Sequence();
					seq.SetTarget(renderer);
					seq.AppendInterval(FlashHold);
					seq.Append(renderer.material.DOAnimateFloat(FlashName, 0f, FlashOut));
				}
			}

			if (DoesBlink)
			{
				StopAllCoroutines();
				StartCoroutine(BlinkSequence());
			}
		}

		private void OnDeath(DamageInstance dmgInstance)
		{
			Audio.Play(DeathSFX, transform.position);

			if (DeathVFX != null)
			{
				if (DirectionFieldVFX.IsEnabled)
				{
					DeathVFX.SetVector2(DirectionFieldVFX.Value, dmgInstance.Info.Direction);
				}

				DeathVFX.Play();
			}
		}

		private IEnumerator BlinkSequence()
		{
			for (int i = 0; i < BlinkCount; i++)
			{
				foreach (var renderer in Renderers)
				{
					renderer.enabled = true;
				}

				if (i == BlinkCount - 1)
				{
					break;
				}

				yield return new WaitForSeconds(BlinkVisible);

				foreach (var renderer in Renderers)
				{
					renderer.enabled = false;
				}

				yield return new WaitForSeconds(BlinkInvisible);
			}
		}
	}
}
