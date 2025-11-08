using FMODUnity;
using Quinn.MissileSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

namespace Quinn.PlayerSystem
{
	public class Gun : MonoBehaviour
	{
		public MissileSpawnData Missile;
		public bool IsContinuousFire = true;
		public float FireInterval = 0.5f;

		[Space]

		public Sprite AmmoUI;
		public int MagazineSize = 20;
		public int ConsumePerShot = 1;
		public float ReloadTime = 2f;

		[Space]

		[Required]
		public Transform Muzzle;
		[Required]
		public Light2D MuzzleLight;
		[ColorUsage(false, true)]
		public Color MuzzleFlashColor = Color.white;
		[Required]
		public VisualEffect MuzzleVFX;

		[Space]

		public float RecoilOffset = 0.5f;
		public float RecoilRecoveryTime = 1f;

		[Space]

		public EventReference FireSound;
		public EventReference DryFireSound;
		public EventReference ReloadSound;
	}
}
