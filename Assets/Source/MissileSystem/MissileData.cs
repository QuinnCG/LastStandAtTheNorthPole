using FMODUnity;
using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.MissileSystem
{
	[CreateAssetMenu]
	public class MissileData : ScriptableObject
	{
		[Header("Custom Behaviors"), SerializeReference, InlineProperty, HideLabel]
		public CustomMissileBehavior[] CustomBehaviors = System.Array.Empty<CustomMissileBehavior>();

		[Header("Visuals")]
		public Sprite Sprite;
		public Sprite[] RandomSprites;
		public float CollisionRadius = 0.3f;
		public float TrailSpawnRate = 8f;

		[Space]

		public bool HasLight = true;
		public float LightRadius = 5f;
		public float LightIntensity = 1f;
		public Color LightColor = Color.white;

		[Space]

		[AssetsOnly]
		public GameObject TrailVFX;
		public float TrailLifePostDetach = 3f;
		[AssetsOnly]
		public GameObject DeathVFX;

		[Space]

		public EventReference DeathSound, HitSound;

		[Space, Header("Behavior")]

		public Team Team;
		public float Damage = 10f;
		public float Knockback = 6f;
		public StatusEffectInstance[] StatusEffects;

		[Space]

		public bool RotateToVelocity;
		public bool DestoryOnCharacterHit = true;
		public bool DestoryOnObstacleHit = true;

		[Space]

		[HideLabel, InlineProperty]
		public MissileBehavior Behavior;
	}
}
