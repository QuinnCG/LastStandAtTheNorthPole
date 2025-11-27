using Quinn.DamageSystem;
using Quinn.MovementSystem;

namespace Quinn.PlayerSystem.Upgrades
{
    public class StatUpgrade : Upgrade
    {
        public float SpeedAddend = 0f;
        public float DamageAddend = 0f;
        public int HealthAddend = 0;

        public override void ApplyUpgrade()
        {
            if (SpeedAddend > 0f)
            {
				var movement = Player.Instance.GetComponent<CharacterMovement>();
				movement.IncreaseMoveSpeed(SpeedAddend);
			}

            if (DamageAddend > 0f)
            {
                GunManager.Instance.DamageMultiplier += DamageAddend;
			}

            if (HealthAddend > 0)
            {
                Player.Instance.GetComponent<Health>().IncreaseMaxHP(HealthAddend);
			}
		}
    }
}
