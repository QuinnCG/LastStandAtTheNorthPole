using FMODUnity;
using Quinn.MissileSystem;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	[CreateAssetMenu]
	public class Gun : ScriptableObject
	{
		public Sprite Sprite;
		public Sprite AmmoUI;

		public Vector2 MuzzleOffset;
		public MissileSpawnData Missile;

		public float FireRate = 0.5f;
		public int MagazineSize = 20;
		public float ReloadTime = 2f;

		public EventReference FireSound, DryFireSound;
		public EventReference ReloadStartSound, ReloadFinishSound;
	}
}
