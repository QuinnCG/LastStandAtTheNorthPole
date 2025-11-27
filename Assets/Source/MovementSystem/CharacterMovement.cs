using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn.MovementSystem
{
	[RequireComponent(typeof(Health))]
	public class CharacterMovement : Locomotion, IDirection
	{
		[SerializeField, Unit(Units.MetersPerSecond)]
		private float DefaultMoveSpeed = 5f;
		[SerializeField, Unit(Units.MetersPerSecond)]
		private float KnockbackDecayRate = 32f;

		public Vector2 Velocity => Rigidbody.linearVelocity;
		public float RealSpeed => Velocity.magnitude;
		public Vector2 Knockback { get; private set; }

		public bool IsMoving { get; private set; }
		public float MoveSpeed { get; private set; }

		public float FacingDirection { get; private set; } = 1f;
		public Vector2 LastMoveDirection { get; private set; } = Vector2.up;

		private readonly Dictionary<object, float> _moveSpeedFacotors = new();
		private bool _wasMoving;

		protected override void Awake()
		{
			base.Awake();
			ResetMoveSpeed();

			GetComponent<Health>().OnDamaged += OnDamage;
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();

			if (!_wasMoving)
			{
				IsMoving = false;
			}
			else
			{
				_wasMoving = false;
			}

			if (Knockback.sqrMagnitude > 0f)
			{
				float mag = Knockback.magnitude;
				mag = Mathf.Max(0f, mag - (KnockbackDecayRate * Time.deltaTime));
				Knockback = Knockback.normalized * mag;

				AddVelocity(Knockback);
			}
		}

		public void SetMoveSpeed(float speed)
		{
			MoveSpeed = speed;
		}

		public void IncreaseMoveSpeed(float addend)
		{
			MoveSpeed += addend;
		}

		/// <summary>
		/// Set <see cref="MoveSpeed"/> to <see cref="DefaultMoveSpeed"/>.
		/// </summary>
		public void ResetMoveSpeed()
		{
			SetMoveSpeed(DefaultMoveSpeed);
		}

		public void SetFacingDirection(float xDir)
		{
			if (xDir != 0f)
			{
				FacingDirection = Mathf.Sign(xDir);

				var scale = transform.localScale;
				scale.x = Mathf.Abs(scale.x) * FacingDirection;
				transform.localScale = scale;
			}
		}

		/// <summary>
		/// Apply <paramref name="moveDir"/> * <paramref name="speed"/> to the cumulative velocity for this frame.<br/>
		/// All other movement functions make use of this method, directly or indirectly.
		/// </summary>
		/// <param name="moveDir">This value will be normalized automatically.</param>
		public void Move(Vector2 moveDir, float speed, bool setFacingDir = true)
		{
			if (moveDir.sqrMagnitude == 0f || speed <= 0f)
				return;

			moveDir.Normalize();
			LastMoveDirection = moveDir;

			foreach (var factor in _moveSpeedFacotors.Values)
			{
				speed *= factor;
			}

			AddVelocity(moveDir * speed);
			IsMoving = _wasMoving = true;

			if (setFacingDir)
			{
				SetFacingDirection(moveDir.x);
			}
		}

		/// <summary>
		/// Apply <see cref="MoveSpeed"/> * <paramref name="moveDir"/> to the cumulative velocity for this frame.
		/// </summary>
		/// <param name="moveDir">This value will be normalized automatically.</param>
		public void Move(Vector2 moveDir, bool setFacingDir = true)
		{
			Move(moveDir, MoveSpeed, setFacingDir);
		}

		/// <summary>
		/// Move towards the given destination at the speed of <paramref name="moveSpeed"/>.<br/>
		/// The method will try to avoid overshooting the destination.
		/// </summary>
		/// <param name="destination">The destination to reach.</param>
		/// <param name="stoppingDst">If we are within this distance of the destination, then we have reached the destination.</param>
		/// <returns>True, when the destination has been reached.</returns>
		public bool MoveTo(Vector2 destination, float moveSpeed, float stoppingDst = 0.1f, bool setFacingDir = true)
		{
			Vector2 delta = destination - (Vector2)transform.position;
			float dt = Time.deltaTime;

			float dstLeft = delta.magnitude;
			float dstThisFrame = moveSpeed * dt;

			float finalDst = Mathf.Min(dstLeft, dstThisFrame);
			float speed = finalDst / dt;

			// Delta will be automatically normalized into a directional vector.
			Move(delta, speed, setFacingDir);

			return dstLeft < stoppingDst;
		}
		/// <summary>
		/// Move towards the given destination at the speed of <see cref="MoveSpeed"/>.<br/>
		/// Stop when reaching it.
		/// </summary>
		/// <param name="destination">The destination to reach.</param>
		/// <param name="stoppingDst">If we are within this distance of the destination, then we have reached the destination.</param>
		/// <returns>True, when the destination has been reached.</returns>
		public bool MoveTo(Vector2 destination, float stoppingDst = 0.1f, bool setFacingDir = true)
		{
			return MoveTo(destination, MoveSpeed, stoppingDst, setFacingDir);
		}

		/// <summary>
		/// Knockback is applied once and will slowly decay over time.<br/>
		/// Knockback instances can be stacked.
		/// </summary>
		/// <param name="velocity">The initial velocity of the knockback.</param>
		public void ApplyKnockback(Vector2 velocity)
		{
			Knockback += velocity;
		}

		public void AddMoveSpeedFactor<T>(T key, float factor) where T : class
		{
			if (_moveSpeedFacotors.ContainsKey(key))
			{
				_moveSpeedFacotors[key] = factor;
			}
			else
			{
				_moveSpeedFacotors.Add(key, factor);
			}
		}

		public void RemoveMoveSpeedFactor<T>(T key) where T : class
		{
			_moveSpeedFacotors.Remove(key);
		}

		private void OnDamage(DamageInstance dmgInstance)
		{
			var info = dmgInstance.Info;

			if (info.Knockback > 0f)
			{
				ApplyKnockback(info.Knockback * info.Direction);
			}
		}
    }
}
