using Mono.Cecil.Cil;
using Quinn.DamageSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Quinn.MissileSystem
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class Missile : MonoBehaviour
	{
		[SerializeField]
		private Light2D Light;

		public LayerMask CompositeMask { get; private set; }
		public LayerMask CharacterMask { get; private set; }

		public MissileData Data { get; private set; }
		public Vector2 Direction { get; private set; }

		public event System.Action OnDeath;

		private Vector2 _lastPhysicsPos;
		private Transform _trail;

		private void OnDestroy()
		{
			OnDeath?.Invoke();
		}

		public void Init(Vector2 dir, MissileData data)
		{
			Data = data;
			Direction = dir.normalized;

			var renderer = GetComponent<SpriteRenderer>();
			renderer.enabled = data.Sprite != null;
			renderer.sprite = data.Sprite;

			_lastPhysicsPos = transform.position;

			if (Data.TrailVFX != null)
			{
				_trail = Data.TrailVFX.Clone(transform).transform;
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
		}

		public void PhysicsUpdate()
		{
			Vector2 deltaPos = (Vector2)transform.position - _lastPhysicsPos;
			var hit = Physics2D.CircleCast(_lastPhysicsPos, Data.CollisionRadius, deltaPos.normalized, deltaPos.magnitude, CompositeMask);
			_lastPhysicsPos = transform.position;

			if (hit.collider != null)
			{
				int colliderMask = LayerMask.GetMask(LayerMask.LayerToName(hit.collider.gameObject.layer));
				bool destroy = false;

				if (Data.DestoryOnCharacterHit && (colliderMask & CharacterMask) > 0)
				{
					var dmgInfo = new DamageInfo()
					{
						Damage = Data.Damage,
						Knockback = Data.Knockback,
						Element = Data.Element,
						Direction = deltaPos.normalized,
						Team = Data.Team
					};

					if (hit.collider.TryGetComponent(out IDamageable dmg) && dmg.TryTakeDamage(dmgInfo))
					{
						destroy = true;
					}
				}

				if (Data.DestoryOnObstacleHit && (colliderMask & Layers.ObstacleMask) > 0)
				{
					destroy = true;
				}

				if (destroy)
				{
					Kill();
				}
			}
		}

		public void SetDirection(Vector2 dir)
		{
			Direction = dir.normalized;
		}

		public bool GetHomingTarget(out Vector2 pos)
		{
			var results = Physics2D.OverlapCircleAll(transform.position, Data.Behavior.MaxHomingDistance, CharacterMask);
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
			Audio.Play(Data.DeathSound, transform.position);

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
