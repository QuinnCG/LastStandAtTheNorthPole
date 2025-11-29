using DG.Tweening;
using QFSW.QC;
using Quinn.MissileSystem;
using Quinn.MovementSystem;
using Quinn.PlayerSystem.Upgrades;
using Quinn.WaveSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	[RequireComponent(typeof(CharacterMovement))]
	public class GunManager : MonoBehaviour
	{
		public static GunManager Instance { get; private set; }

		[SerializeField]
		private float StatMultiplierDivisor = 10f;

		[Space]

		[SerializeField]
		private float MuzzleFlashFadeTime = 0.1f;
		[SerializeField]
		private Ease MuzzleFlashEase = Ease.Linear;

		[Space]

		[SerializeField, Required]
		private Transform GunHandle;

		public Gun? Equipped { get; private set; }
		public bool IsFiring { get; private set; }
		public int Magazine { get; private set; }

		public event System.Action<Gun>? OnEquipped;
		public event System.Action? OnUnequipped;

		/// <summary>
		/// A multiplier based on wave index.
		/// </summary>
		public float CurrentWaveStatMultiplier
		{
			get
			{
				int wave = WaveManager.Instance.WaveNumber;
				float index = (wave - 1) / StatMultiplierDivisor;
				return Mathf.Pow(wave, index);
			}
		}
		/// <summary>
		/// A multiplier based on upgrades.
		/// </summary>
		public float DamageMultiplier { get; set; } = 1f;

		public UpgradeSO? EquippedGunUpgradeSO { get; private set; }
		public float EquippedMultiplier { get; private set; } = 1f;

		private CharacterMovement _movement;
		private float _nextFireTime;


		private void Awake()
		{
			Instance = this;
			_movement = GetComponent<CharacterMovement>();
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

		/// <summary>
		/// Equips a gun after deleting the current. Also, reloads ammo.
		/// </summary>
		/// <param name="gun">The prefab of the gun. This will be cloned.</param>
		public void Equip(UpgradeSO gunUpgradeSO)
		{
			if (Equipped != null)
			{
				Unequip();
			}

#if UNITY_EDITOR
			if (gunUpgradeSO.Upgrade is null)
			{
				Log.Error($"Gun asset '{gunUpgradeSO.name}' is missing a reference to itself!");
			}
#endif

			var gun = gunUpgradeSO.Upgrade as WeaponUpgrade;
			EquippedGunUpgradeSO = gunUpgradeSO;

			Equipped = gun!.GunPrefab!.gameObject.Clone<Gun>(GunHandle);
			ReplenishMagazine();

			EquippedMultiplier = CurrentWaveStatMultiplier;
			OnEquipped?.Invoke(gun.GunPrefab!);
		}

		public void Unequip()
		{
			if (Equipped != null)
			{
				Equipped.gameObject.Destroy();
			}

			EquippedGunUpgradeSO = null;
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

		private void TryFire()
		{
			if (Player.Instance.IsDashing)
				return;

			if (Equipped == null)
			{
				Log.Error($"No gun is equipped!");
				return;
			}

			var origin = GetMissileSpawnPoint();
			var dir = CrosshairManager.Direction;

			if (Time.time >= _nextFireTime)
			{
				_nextFireTime = Time.time + (Equipped!.FireInterval / CurrentWaveStatMultiplier);
			}

			if (Magazine > 0)
			{
				MissileManager.Instance.Spawn(origin, dir, Equipped.Missile.Missile!, Equipped.Missile.Pattern, damageFactor: DamageMultiplier * EquippedMultiplier);
				Recoil(Equipped.RecoilOffset, Equipped.RecoilRecoveryTime);

				Audio.Play(Equipped.FireSound, origin);
				Equipped.MuzzleLight.color = Equipped.MuzzleFlashColor;
				Equipped.MuzzleLight.DOFade(0f, MuzzleFlashFadeTime)
					.SetEase(MuzzleFlashEase);

				Magazine = Mathf.Max(0, Magazine - Equipped.ConsumePerShot);

				_movement.ApplyKnockback(-dir * Equipped.SelfKnockback);

				Equipped.MuzzleVFX.Play();
			}
			else
			{
				Audio.Play(Equipped.DryFireSound, Equipped.Muzzle.transform.position);
			}
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

		[Command("wave.stat")]
		protected void GetStatMultiplier()
		{
			Log.Info($"Wave Stat Multiplier: {CurrentWaveStatMultiplier:0.00}");
		}
	}
}
