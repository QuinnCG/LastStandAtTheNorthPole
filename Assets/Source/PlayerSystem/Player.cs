using Quinn.MovementSystem;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(CharacterMovement))]
	public class Player : MonoBehaviour
	{
		[SerializeField]
		private float DashSpeed = 12f, DashDistance = 3f, DashCooldown = 0.5f;

		public bool IsDashing { get; private set; }

		private CharacterMovement _movement;
		private float _dashEndTime;
		private float _nextDashAllowedTime;

		private void Awake()
		{
			_movement = GetComponent<CharacterMovement>();
			SetUpBindings();
		}

		private void Update()
		{
			UpdateMove();
			UpdateDash();
		}

		private void UpdateMove()
		{
			if (IsDashing)
				return;

			var inputDir = InputManager.Instance.MoveInputDir;
			_movement.Move(inputDir);
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
			}
		}
	}
}
