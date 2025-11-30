using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

namespace Quinn.MissileSystem
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class Missile : MonoBehaviour
	{
		[SerializeField]
		private Light2D Light;
		[SerializeField, Unit(Units.Meter)]
		private float DistanceTillDeath = 20f;

		public LayerMask CompositeMask { get; private set; }
		public LayerMask CharacterMask { get; private set; }

		public MissileData? Data { get; private set; }
		public Vector2 Direction { get; private set; }
		public float DamageFactor { get; set; } = 1f;

		public event System.Action? OnDeath;

		private Vector2 _startPos;
		private Vector2 _lastPhysicsPos;
		private Transform? _trail;

		private void OnDestroy()
		{
			OnDeath?.Invoke();
		}

        private void LateUpdate()
        {
			if (Data!.RotateToVelocity)
			{
				Vector2 deltaPos = (Vector2)transform.position - _lastPhysicsPos;
				deltaPos.Normalize();
				float angle = Mathf.Atan2(deltaPos.y, deltaPos.x) * Mathf.Rad2Deg;

				transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			}

			foreach (var behavior in Data!.CustomBehaviors)
			{
				behavior.OnUpdate();
			}
		}

        public void Init(Vector2 dir, MissileData data)
		{
			_startPos = transform.position;

			Data = data;
			Direction = dir.normalized;

			Sprite sprite;

			if (data.RandomSprites != null && data.RandomSprites.Length > 0)
			{
				sprite = data.RandomSprites.GetRandom();
			}
			else
			{
				sprite = data.Sprite;
			}

			var renderer = GetComponent<SpriteRenderer>();
			renderer.enabled = sprite != null;
			renderer.sprite = sprite;

			_lastPhysicsPos = transform.position;

			if (Data.TrailVFX != null)
			{
				_trail = Data.TrailVFX.Clone(transform).transform;

				var vfx = _trail.GetComponent<VisualEffect>();
				vfx.SetFloat("SpawnRate", data.TrailSpawnRate);
			}

			string charLayer = (Data.Team is Team.Friendly) ? Layers.HostileCharacterName : Layers.FriendlyCharacterName;
			CharacterMask = LayerMask.GetMask(charLayer);
			CompositeMask = LayerMask.GetMask(Layers.ObstacleName, charLayer);

			if (Data.HasLight)
			{
				Light.intensity = Data.LightIntensity;
				Light.pointLightOuterRadius = Data.LightRadius;
				Light.color = Data.LightColor;
			}
			else
			{
				Light.Destroy();
			}

			foreach (var behavior in Data.CustomBehaviors)
			{
				behavior.OnSpawn(this);
			}
		}

        public void PhysicsUpdate()
		{
			Vector2 deltaPos = (Vector2)transform.position - _lastPhysicsPos;
			var hit = Physics2D.CircleCast(_lastPhysicsPos, Data!.CollisionRadius, deltaPos.normalized, deltaPos.magnitude, CompositeMask);
			_lastPhysicsPos = transform.position;

			if (hit.collider != null)
			{
				int colliderMask = LayerMask.GetMask(LayerMask.LayerToName(hit.collider.gameObject.layer));
				bool doDestroy = false;
				bool hitObstacle = false;

				if (Data.DestoryOnCharacterHit && (colliderMask & CharacterMask) > 0)
				{
					var dmgInfo = new DamageInfo()
					{
						Damage = Data.Damage * DamageFactor,
						Knockback = Data.Knockback * DamageFactor,
						StatusEffects = Data.StatusEffects,
						Direction = deltaPos.normalized,
						Team = Data.Team
					};

					if (hit.collider.TryGetComponent(out IDamageable dmg) && dmg.TryTakeDamage(dmgInfo))
					{
						doDestroy = true;
						hitObstacle = true;
					}
				}

				if (Data.DestoryOnObstacleHit && (colliderMask & Layers.ObstacleMask) > 0)
				{
					doDestroy = true;
					hitObstacle = true;
				}

				if (doDestroy)
				{
					if (hitObstacle)
					{
						Audio.Play(Data!.HitSound, transform.position);

						foreach (var behavior in Data!.CustomBehaviors)
						{
							behavior.OnHit(hit.collider.gameObject);
						}
					}

					Kill();
					return;
				}
			}

			if (transform.position.DistanceTo(_startPos) > DistanceTillDeath)
			{
				Kill();
			}
		}

		public void SetDirection(Vector2 dir)
		{
			Direction = dir.normalized;
		}

		public bool GetHomingTarget(out Vector2 pos)
		{
			var results = Physics2D.OverlapCircleAll(transform.position, Data!.Behavior.MaxHomingDistance, CharacterMask);
			var nearest = results.GetClosestTo(x => x.transform.position.SquaredDistanceTo(transform.position));

			if (nearest == null)
			{
				pos = default;
				return false;
			}

			pos = nearest.transform.position;
			return true;
		}

		public void Kill()
		{
			foreach (var behavior in Data!.CustomBehaviors)
			{
				behavior.OnDestroy();
			}

			Audio.Play(Data!.DeathSound, transform.position);

			MissileManager.Instance.DestroyMissile(this);

			if (_trail != null)
			{
				_trail.SetParent(null, true);
				_trail.gameObject.Destroy(Data.TrailLifePostDetach);

				_trail = null;
			}

			if (Data.DeathVFX != null)
			{
				Data.DeathVFX.Clone(transform.position);
			}
		}
	}
}
