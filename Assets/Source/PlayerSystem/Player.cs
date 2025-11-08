using FMODUnity;
using Quinn.MovementSystem;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CharacterMovement))]
	[RequireComponent(typeof(GunManager))]
	public class Player : MonoBehaviour
	{
		[SerializeField]
		private float DashSpeed = 12f, DashDistance = 3f, DashCooldown = 0.5f;
		[SerializeField]
		private EventReference DashSound;
		[SerializeField, Required]
		private Transform Origin;
		[SerializeField]
		private float FadeInTime = 1f;

		public bool IsDashing { get; private set; }

		private Animator _animator;
		private CharacterMovement _movement;
		private GunManager _gunManager;

		private float _dashEndTime;
		private float _nextDashAllowedTime;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_movement = GetComponent<CharacterMovement>();
			_gunManager = GetComponent<GunManager>();

			SetUpBindings();
		}

		private async void Start()
		{
			await TransitionManager.Instance.FadeFromBlackAsync(FadeInTime);
		}

		private void Update()
		{
			UpdateMove();
			UpdateDash();
			UpdateAnimation();
		}

		private void LateUpdate()
		{
			if (IsDashing)
			{
				_movement.SetFacingDirection(_movement.LastMoveDirection.x);
			}
			else
			{
				var dir = GetAimDir();
				_movement.SetFacingDirection(dir.x);
			}
		}

		private Vector3 GetAimDir()
		{
			return Origin.position.DirectionTo(CrosshairManager.Position);
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
				float moveDir = Mathf.Sign(_movement.RealVelocity.x);
				float lookDir = Mathf.Sign(GetAimDir().x);

				string moveAnim = "Moving";

				if (_movement.IsMoving && moveDir != lookDir)
				{
					moveAnim = "MovingBack";
				}

				_animator.Play(_movement.IsMoving ? moveAnim : "Idling");
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

			InputManager.Instance.OnFireStart += () => _gunManager.StartFiring();
			InputManager.Instance.OnFireStop += () => _gunManager.StopFiring();
		}

		private void OnDash()
		{
			if (!IsDashing && Time.time > _nextDashAllowedTime)
			{
				IsDashing = true;
				_dashEndTime = Time.time + (DashDistance / DashSpeed);
				_nextDashAllowedTime = _dashEndTime + DashCooldown;

				Audio.Play(DashSound, transform);

				_gunManager.ReplenishMagazine();
			}
		}
	}
}
