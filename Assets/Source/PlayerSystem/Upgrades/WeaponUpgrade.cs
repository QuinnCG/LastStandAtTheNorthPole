using Sirenix.OdinInspector;

namespace Quinn.PlayerSystem.Upgrades
{
    public class WeaponUpgrade : Upgrade
	{
		[AssetsOnly]
		public Gun? GunPrefab;

        public override void ApplyUpgrade()
        {
			GunManager.Instance.Equip(GunPrefab!);
        }
    }
}
