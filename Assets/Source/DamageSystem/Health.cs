using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.DamageSystem
{
	public class Health : MonoBehaviour, IDamageable
	{
		[SerializeField]
		private float Default = 50f;
		[field: SerializeField]
		public Team Team { get; private set; } = Team.Hostile;

		[Space, SerializeField, Unit(Units.Second)]
		private float PostHurtImmunity;
		[SerializeField]
		private bool AllowHalfHeart;
		[SerializeField]
		private EventReference HurtSound, DeathSound;

		public float Current { get; private set; }
		public float Max { get; private set; }
		public float Missing => Mathf.Clamp(Max - Current, 0f, Max);
		public float Normalized => Current / Max;
		public bool IsHalfHeart { get; private set; }

		public bool IsAlive => !IsDead;
		public bool IsDead { get; private set; }

		public event System.Action<DamageInstance> OnDamaged, OnDeath;
		public event System.Action<float> OnHeal;

		private float _nextImmunityEndTime;

		private void Awake()
		{
			Current = Max = Default;
		}

		public bool CanDamage(DamageInfo info)
		{
			if (info.Team == Team)
				return false;

			if (IsDead)
				return false;

			if (PostHurtImmunity > 0f && Time.time < _nextImmunityEndTime)
				return false;

			return true;
		}

		public bool TryTakeDamage(DamageInfo info, bool force = false)
		{
			if (!force && !CanDamage(info))
				return false;

			_nextImmunityEndTime = Time.time + PostHurtImmunity;
			info.Direction.Normalize();

			var dmgInstance = new DamageInstance()
			{
				Info = info,
				FinalDamage = info.Damage,
				RealDamage = Mathf.Min(info.Damage, Current)
			};

			Current = Mathf.Max(0f, Current - dmgInstance.FinalDamage);

			if (!force && AllowHalfHeart && !IsHalfHeart && Current < 0.5f)
			{
				IsHalfHeart = true;
				Current = 0.5f;
			}

			dmgInstance.WasLethal = Current == 0f;
			OnDamaged?.Invoke(dmgInstance);

			Audio.Play(HurtSound, transform.position);

			if (Current == 0f)
			{
				Audio.Play(DeathSound, transform.position);

				IsDead = true;
				OnDeath?.Invoke(dmgInstance);
			}

			return true;
		}

		public void Kill(bool force = false)
		{
			TryTakeDamage(new()
			{
				Damage = Max,
				Team = (Team == Team.Friendly) ? Team.Hostile : Team.Environment
			}, force: force);
		}

		public void Heal(float health)
		{
			IsHalfHeart = false;
			Current = Mathf.Min(Max, Current + health);
			OnHeal?.Invoke(health);
		}

		public void FullHeal()
		{
			Heal(Max);
		}
	}
}
