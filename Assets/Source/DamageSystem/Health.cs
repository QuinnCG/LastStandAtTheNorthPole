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

		public float Current { get; private set; }
		public float Max { get; private set; }
		public float Missing => Mathf.Clamp(Max - Current, 0f, Max);
		public float Normalized => Current / Max;

		public bool IsAlive => !IsDead;
		public bool IsDead { get; private set; }

		public event System.Action<DamageInstance> OnDamage, OnDeath;
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
				FinalDamage = info.Damage
			};

			Current = Mathf.Max(0f, Current - dmgInstance.FinalDamage);

			dmgInstance.WasLethal = Current == 0f;
			OnDamage?.Invoke(dmgInstance);

			if (Current == 0f)
			{
				IsDead = true;
				OnDeath?.Invoke(dmgInstance);
			}

			return true;
		}

		public void Kill()
		{
			TryTakeDamage(new()
			{
				Damage = Max,
				Team = (Team == Team.Friendly) ? Team.Hostile : Team.Environment
			}, force: true);
		}

		public void Heal(float health)
		{
			Current = Mathf.Min(Max, Current + health);
			OnHeal?.Invoke(health);
		}

		public void FullHeal()
		{
			Heal(Max);
		}
	}
}
