using Quinn.DamageSystem;
using Quinn.MovementSystem;
using UnityEngine;

namespace Quinn.PlayerSystem.Upgrades
{
	public class StatUpgrade : Upgrade
	{
		public float SpeedAddend = 0f;
		public float DashDistanceAddend = 0f;
		public float DamageAddend = 0f;
		public int HealthAddend = 0;

		[Space]

		public float HealInterval = 10f;
		public float HealAmount = 0f;

		public override void ApplyUpgrade()
		{
			if (SpeedAddend > 0f)
			{
				var movement = Player.Instance.GetComponent<CharacterMovement>();
				movement.IncreaseMoveSpeed(SpeedAddend);
			}

			if (DashDistanceAddend > 0f)
			{
				Player.Instance.IncreaseDashDistance(DashDistanceAddend);
			}

			if (DamageAddend > 0f)
			{
				GunManager.Instance.DamageMultiplier += DamageAddend;
			}

			if (HealthAddend > 0)
			{
				Player.Instance.GetComponent<Health>().IncreaseMaxHP(HealthAddend);
			}

			if (HealAmount > 0f)
			{
				Player.Instance.SetRegen(HealInterval, HealAmount);
			}
		}
	}
}
