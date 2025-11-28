using Sirenix.OdinInspector;

namespace Quinn.PlayerSystem.Upgrades
{
    public class WeaponUpgrade : Upgrade
	{
		[Required]
		public UpgradeSO? SelfSO;
		[AssetsOnly]
		public Gun? GunPrefab;

        public override void ApplyUpgrade()
        {
			GunManager.Instance.Equip(SelfSO!);
		}
    }
}
