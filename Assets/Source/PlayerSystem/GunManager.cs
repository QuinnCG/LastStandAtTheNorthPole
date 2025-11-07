using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	public class GunManager : MonoBehaviour
	{
		[SerializeField, Required]
		private SpriteRenderer GunSprite;
		[SerializeField, Required]
		private Transform MuzzleTransform;

		public Gun Equipped { get; private set; }
		public bool IsFiring { get; private set; }
		public int Magazine { get; private set; }

		public event System.Action<Gun> OnEquipped;
		public event System.Action OnUnequipped;

		public void Equip(Gun gun)
		{
			Equipped = gun;
			GunSprite.sprite = gun.Sprite;
			ReplenishMagazine();
			OnEquipped?.Invoke(gun);
		}

		public void Unequip()
		{
			GunSprite.sprite = null;
			Equipped = null;
			OnUnequipped?.Invoke();
		}

		public void ReplenishMagazine()
		{
			if (Equipped != null)
			{
				Magazine = Equipped.MagazineSize;
				Audio.Play(Equipped.ReloadFinishSound, transform);
			}
		}

		public void StartFiring()
		{
			if (!IsFiring)
			{
				IsFiring = true;
			}
		}

		public void StopFiring()
		{
			if (IsFiring)
			{
				IsFiring = false;
			}
		}

		private void Recoil(float offset, float recoveryTime)
		{
			GunOrbiter.Instance.Recoil(offset, recoveryTime);
		}
	}
}
