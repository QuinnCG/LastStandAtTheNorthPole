using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn.DamageSystem
{
	[RequireComponent(typeof(Collider2D))]
	public class DamageOnContact : MonoBehaviour
	{
		[SerializeField]
		private float Damage = 1f;
		[SerializeField]
		private float Knockback = 8f;
		[SerializeField]
		private StatusEffectInstance[] Statuses;

		[Space]

		[SerializeField, ShowIf("@GetComponent<Health>() == null")]
		private Team Team;

		public event System.Action? OnContact;

		private Collider2D _collider;
		private Team _team;

		private void Awake()
		{
			_collider = GetComponent<Collider2D>();

			if (TryGetComponent(out Health health))
			{
				_team = health.Team;
			}
			else
			{
				_team = Team;
			}
		}

		private void FixedUpdate()
		{
			var results = new List<Collider2D>();
			Physics2D.OverlapCollider(_collider, new ContactFilter2D()
			{
				layerMask = ~0,
				useLayerMask = true
			}, results);

			foreach (var collider in results)
			{
				if (collider == _collider)
					continue;

				if (collider.TryGetComponent(out Health health))
				{
					bool success = health.TryTakeDamage(new DamageInfo()
					{
						Damage = Damage,
						Team = _team,
						StatusEffects = Statuses,
						Direction = transform.position.DirectionTo(health.transform.position),
						Knockback = Knockback
					});

					if (success)
					{
						OnContact?.Invoke();
					}
				}
			}
		}
	}
}
