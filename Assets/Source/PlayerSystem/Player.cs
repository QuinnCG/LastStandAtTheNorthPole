using FMODUnity;
using QFSW.QC;
using Quinn.DamageSystem;
using Quinn.MovementSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CharacterMovement))]
	[RequireComponent(typeof(GunManager))]
	[RequireComponent(typeof(Collider2D))]
	public class Player : MonoBehaviour
	{
		public static Player Instance { get; private set; }

		public Bounds Hitbox => _hitbox.bounds;
		public Vector2 Velocity => _movement.Velocity;

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
		private Collider2D _hitbox;

		private float _dashEndTime;
		private float _nextDashAllowedTime;

		private void Awake()
		{
			Instance = this;

			_animator = GetComponent<Animator>();
			_movement = GetComponent<CharacterMovement>();
			_gunManager = GetComponent<GunManager>();
			_hitbox = GetComponent<Collider2D>();

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
				float moveDir = Mathf.Sign(_movement.Velocity.x);
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

		[Command("hurt")]
		protected void Hurt_Cmd(float dmg = 1f)
		{
			float angle = Random.Range(0f, Mathf.PI * 2f);

			GetComponent<Health>().TryTakeDamage(new()
			{
				Damage = dmg,
				Direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)),
				Knockback = Random.Range(5f, 18f),
				Team = Team.Environment
			});
		}

		[Command("kill")]
		protected void Kill_Cmd()
		{
			GetComponent<Health>().Kill(true);
		}

		[Command("heal")]
		protected void Heal_Cmd(float health = 1f)
		{
			GetComponent<Health>().Heal(health);
		}

		[Command("fullheal")]
		protected void FullHeal_Cmd()
		{
			GetComponent<Health>().FullHeal();
		}
	}
}
