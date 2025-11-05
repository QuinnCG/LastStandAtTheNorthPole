using FMODUnity;
using Quinn.MovementSystem;
using System;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CharacterMovement))]
	public class Player : MonoBehaviour
	{
		[SerializeField]
		private float DashSpeed = 12f, DashDistance = 3f, DashCooldown = 0.5f;
		[SerializeField]
		private EventReference DashSound;

		public bool IsDashing { get; private set; }

		private Animator _animator;
		private CharacterMovement _movement;

		private float _dashEndTime;
		private float _nextDashAllowedTime;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_movement = GetComponent<CharacterMovement>();

			SetUpBindings();
		}

		private void Update()
		{
			UpdateMove();
			UpdateDash();
			UpdateAnimation();
		}

		private void UpdateMove()
		{
			if (IsDashing)
				return;

			var inputDir = InputManager.Instance.MoveInputDir;
			_movement.Move(inputDir);
		}

		private void UpdateAnimation()
		{
			if (IsDashing)
			{
				_animator.Play("Dashing");
			}
			else
			{
				_animator.Play(_movement.IsMoving ? "Moving" : "Idling");
			}
		}

		private void UpdateDash()
		{
			if (IsDashing)
			{
				if (Time.time > _dashEndTime)
				{
					IsDashing = false;
					return;
				}

				var inputDir = _movement.LastMoveDirection;
				_movement.AddVelocity(inputDir * DashSpeed);
			}
		}

		private void SetUpBindings()
		{
			InputManager.Instance.OnDash += OnDash;
		}

		private void OnDash()
		{
			if (!IsDashing && Time.time > _nextDashAllowedTime)
			{
				IsDashing = true;
				_dashEndTime = Time.time + (DashDistance / DashSpeed);
				_nextDashAllowedTime = _dashEndTime + DashCooldown;

				Audio.Play(DashSound, transform);
			}
		}
	}
}
