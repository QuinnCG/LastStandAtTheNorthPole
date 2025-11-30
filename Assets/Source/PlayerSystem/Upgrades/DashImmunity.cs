namespace Quinn.PlayerSystem.Upgrades
{
	public class DashImmunity : Upgrade
	{
		public override void ApplyUpgrade()
		{
			Player.Instance.IsImmuneDuringDash = true;
		}
	}
}
