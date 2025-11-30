using FMODUnity;
using QFSW.QC;
using Quinn.DamageSystem;
using Quinn.MovementSystem;
using Quinn.UI;
using Quinn.WaveSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

namespace Quinn.PlayerSystem
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CharacterMovement))]
	[RequireComponent(typeof(GunManager))]
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Health))]
	[RequireComponent(typeof(GunOrbiter))]
	[RequireComponent(typeof(CrosshairManager))]
	public class Player : MonoBehaviour
	{
		public static Player Instance { get; private set; }
		private static readonly object _blockInputKey = new();

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
		[SerializeField]
		private float DeathFadeOutTime = 3.6f;
		[SerializeField, Required]
		private Light2D[] AuraLights;
		[SerializeField, Required]
		private VisualEffect SnowVFX;
		[SerializeField]
		private float SnowSpawnRatePerWaveFactor = 16f, MaxSnowSpawnRate = 300f;

		public bool IsDashing { get; private set; }

		private Animator _animator;
		private CharacterMovement _movement;
		private GunManager _gunManager;
		private Collider2D _hitbox;
		private Health _health;

		private float _dashEndTime;
		private float _nextDashAllowedTime;

		private void Awake()
		{
			Instance = this;
			WaveManager.Instance.ResetWave();

			_animator = GetComponent<Animator>();
			_movement = GetComponent<CharacterMovement>();
			_gunManager = GetComponent<GunManager>();
			_hitbox = GetComponent<Collider2D>();
			_health = GetComponent<Health>();

			SetUpBindings();
			_health.OnDeath += OnDeath;
		}

		private async void Start()
		{
			WaveManager.Instance.StartNextWave();

			MusicManager.Instance.Play();
			InputManager.Instance.UnblockInput(_blockInputKey);
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
			if (_health.IsDead)
				return;

			if (IsDashing)
			{
				_movement.SetFacingDirection(_movement.LastMoveDirection.x);
			}
			else
			{
				var dir = GetAimDir();
				_movement.SetFacingDirection(dir.x);
			}

			SnowVFX.SetFloat("BaseSpawnRate", Mathf.Min(WaveManager.Instance.WaveNumber * SnowSpawnRatePerWaveFactor, MaxSnowSpawnRate));
		}

		private void OnDestroy()
		{
			if (InputManager.Instance == null)
				return;

			InputManager.Instance.OnDash -= OnDash;

			InputManager.Instance.OnFireStart -= OnFireStart;
			InputManager.Instance.OnFireStop -= OnFireStop;
		}

		public void SetSnowSpawnFactor(float factor)
		{
			SnowVFX.SetFloat("SpawnFactor", factor);
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
			if (_health.IsDead)
				return;

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
					StopDashing();
					return;
				}

				var inputDir = _movement.LastMoveDirection;
				_movement.AddVelocity(inputDir * DashSpeed);
			}
		}

		private void StopDashing()
		{
			IsDashing = false;
			_health.UnblockDamage(this);
		}

		private void SetUpBindings()
		{
			InputManager.Instance.OnDash += OnDash;

			InputManager.Instance.OnFireStart += OnFireStart;
			InputManager.Instance.OnFireStop += OnFireStop;
		}

		private void OnFireStart()
		{
			if (_health.IsDead)
				return;

			_gunManager.StartFiring();
		}

		private void OnFireStop()
		{
			if (_health.IsDead)
				return;

			_gunManager.StopFiring();
		}

		private void OnDash()
		{
			if (!IsDashing && Time.time > _nextDashAllowedTime)
			{
				IsDashing = true;
				_dashEndTime = Time.time + (DashDistance / DashSpeed);
				_nextDashAllowedTime = _dashEndTime + DashCooldown;

				_gunManager.ReplenishMagazine();
				_health.BlockDamage(this);

				Audio.Play(DashSound, transform);
			}
		}

		private async void OnDeath(DamageInstance dmgInstance)
		{
			UpgradeUI.Instance.Hide();

			MusicManager.Instance.Pause();

			_gunManager.StopFiring();
			StopDashing();

			InputManager.Instance.BlockInput(_blockInputKey);

			GetComponent<GunOrbiter>().HideGun();
			GetComponent<CrosshairManager>().HideCrosshair();

			_animator.Play("Death");

			foreach (var light in AuraLights)
			{
				light.DOFade(0f, DeathFadeOutTime);
			}

			await TransitionManager.Instance.FadeToBlackAsync(DeathFadeOutTime);

			// Load menu scene (2nd index).
			await SceneManager.LoadSceneAsync(1);
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
