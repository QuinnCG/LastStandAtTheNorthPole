using DG.Tweening;
using Quinn.MissileSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	public class GunManager : MonoBehaviour
	{
		public static GunManager Instance { get; private set; }

		[SerializeField]
		private float MuzzleFlashFadeTime = 0.1f;
		[SerializeField]
		private Ease MuzzleFlashEase = Ease.Linear;

		[Space]

		[SerializeField, Required]
		private Transform GunHandle;
		[SerializeField, AssetsOnly]
		private Gun TestingGun;

		public Gun Equipped { get; private set; }
		public bool IsFiring { get; private set; }
		public int Magazine { get; private set; }

		public event System.Action<Gun> OnEquipped;
		public event System.Action OnUnequipped;

		private float _nextFireTime;

		private void Awake()
		{
			Instance = this;

#if UNITY_EDITOR
			if (TestingGun != null)
			{
				Equip(TestingGun);
			}
#endif
		}

		private void Update()
		{
			if (IsFiring && Equipped != null)
			{
				if (Time.time > _nextFireTime)
				{
					TryFire();

					if (!Equipped.IsContinuousFire)
					{
						StopFiring();
					}
				}
			}
		}

		public void Equip(Gun gun)
		{
			if (Equipped != null)
			{
				Unequip();
			}

			Equipped = gun.gameObject.Clone<Gun>(GunHandle);
			ReplenishMagazine();
			OnEquipped?.Invoke(gun);
		}

		public void Unequip()
		{
			if (Equipped != null)
			{
				Equipped.gameObject.Destroy();
			}

			Equipped = null;
			OnUnequipped?.Invoke();
		}

		public void ReplenishMagazine()
		{
			if (Equipped != null && Magazine < Equipped.MagazineSize)
			{
				Magazine = Equipped.MagazineSize;
				Audio.Play(Equipped.ReloadSound, transform);
			}
		}

		public void StartFiring()
		{
			if (!IsFiring)
			{
				IsFiring = true;
				TryFire();
			}
		}

		public void StopFiring()
		{
			if (IsFiring)
			{
				IsFiring = false;
			}
		}

		private bool TryFire()
		{
			if (Player.Instance.IsDashing)
				return false;

			_nextFireTime = Time.time + Equipped.FireInterval;

			var origin = GetMissileSpawnPoint();
			var dir = CrosshairManager.Direction;

			if (Magazine > 0)
			{
				MissileManager.Instance.Spawn(origin, dir, Equipped.Missile.Missile, Equipped.Missile.Pattern);
				Recoil(Equipped.RecoilOffset, Equipped.RecoilRecoveryTime);

				Audio.Play(Equipped.FireSound, origin);
				Equipped.MuzzleLight.color = Equipped.MuzzleFlashColor;
				Equipped.MuzzleLight.DOFade(0f, MuzzleFlashFadeTime)
					.SetEase(MuzzleFlashEase);

				Magazine = Mathf.Max(0, Magazine - Equipped.ConsumePerShot);
			}
			else
			{
				Audio.Play(Equipped.DryFireSound, Equipped.Muzzle.transform.position);
			}

			return true;
		}

		private Vector2 GetMissileSpawnPoint()
		{
			if (Equipped == null)
				return transform.position;

			return Equipped.Muzzle.position;
		}

		private void Recoil(float offset, float recoveryTime)
		{
			GunOrbiter.Instance.Recoil(offset, recoveryTime);
		}
	}
}
