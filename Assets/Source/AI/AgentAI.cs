using Quinn.DamageSystem;
using Quinn.MovementSystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Health))]
	[RequireComponent(typeof(CharacterMovement))]
	public class AgentAI : MonoBehaviour
	{
		[field: SerializeField, Range(0f, 50f)]
		public float Threat { get; private set; } = 10f;
		[field: SerializeField]
		public float DisengageDistance { get; private set; } = 12f;
		[field: SerializeField, Range(0f, 1f)]
		public float DisengageBelowHP { get; private set; } = 0.2f;
		[SerializeField]
		private Vector2 DisengagedRange = new(8f, 10f);

		[Space]

		[SerializeField]
		private float ActRange = 8f;
		[SerializeField]
		private Vector2 IdleDurationWhenInRange = new(2f, 6f);

		[Space]

		[SerializeField, Unit(Units.Degree)]
		private float OrbitArc = 180f;
		[SerializeField]
		private float OrbitFrequency = 0.5f;
		[SerializeField, Range(-1f, 1f)]
		private float OrbitFrequencyFloor = -0.5f;

		[Space]

		[SerializeField]
		private Vector2 OscillateRange = new(2f, 4f);
		[SerializeField]
		private float OscillateFrequency = 0.1f;
		[SerializeField, Range(0f, 1f)]
		private float OscillateTimeOffset = 0.5f;

		public bool IsEngaged { get; private set; }

		protected Animator Animator { get; private set; }
		protected Health Health { get; private set; }
		protected CharacterMovement Movement { get; private set; }
		protected Transform PlayerTransform { get; private set; }

		protected bool HasActiveSequence => _activeSequence != null;
		protected Vector2 DirToPlayer => transform.position.DirectionTo(PlayerTransform.position);
		protected float DstToPlayer => transform.position.DistanceTo(PlayerTransform.position);

		private readonly HashSet<AISequence> _sequences = new();

		private Coroutine? _activeRoutine;
		private IEnumerator? _activeSequence;
		private float _idleEndTime;

		protected virtual void Awake()
		{
			Animator = GetComponent<Animator>();
			Health = GetComponent<Health>();
			Movement = GetComponent<CharacterMovement>();

			Health.OnDamaged += OnDamaged;
			Health.OnDeath += info =>
			{
				AIManager.Instance.Unregister(this);
				OnDeath(info);
			};

			OnRegisterSequences();

			_idleEndTime = Time.time + IdleDurationWhenInRange.GetRandom();
		}

		protected virtual void Start()
		{
			PlayerTransform = PlayerSystem.Player.Instance.transform;
			AIManager.Instance.Register(this);
		}

		protected virtual void Update()
		{
			OnUpdate();

			if (!IsEngaged)
			{
				OrbitPlayer(DisengagedRange.x, DisengagedRange.y);
				return;
			}

			if (HasActiveSequence)
			{
				return;
			}

			if (DstToPlayer < ActRange && Time.time > _idleEndTime && _sequences.Count > 0)
			{
				StartRandomSequence();
			}
			else
			{
				OrbitPlayer(OscillateRange.x, OscillateRange.y);
			}
		}

		public void Engage()
		{
			if (!IsEngaged)
			{
				IsEngaged = true;
			}
		}

		public void Disengage()
		{
			if (IsEngaged)
			{
				IsEngaged = false;
			}
		}

		public virtual bool WantsToDisengage()
		{
			return DstToPlayer > DisengageDistance || Health.Normalized <= DisengageBelowHP;
		}

		protected virtual void OnRegisterSequences() { }

		protected virtual void OnUpdate() { }

		protected void PlaySequence(IEnumerator sequence)
		{
			if (_activeRoutine != null)
			{
				StopActiveSequence();
			}

			_activeSequence = sequence;
			_activeRoutine = StartCoroutine(sequence);
			StartCoroutine(StopSequenceOnFinish(_activeRoutine));
		}

		private IEnumerator StopSequenceOnFinish(Coroutine sequence)
		{
			yield return sequence;

			_idleEndTime = Time.time + IdleDurationWhenInRange.GetRandom();
			StopActiveSequence();
		}

		protected void StopActiveSequence()
		{
			StopCoroutine(_activeRoutine);
			_activeRoutine = null;
			_activeSequence = null;
		}

		protected void FacePlayer()
		{
			Movement.SetFacingDirection(GetDirectionToPlayer(transform.position).x);
		}

		protected void RegisterSequence(System.Func<IEnumerator> callback, float weight = 100f, float cooldown = 0f)
		{
			_sequences.Add(new AISequence(callback, weight, cooldown));
		}

		protected Vector2 GetDirectionToPlayer(Vector2 origin)
		{
			return origin.DirectionTo(PlayerSystem.Player.Instance.Hitbox.center);
		}

		protected Vector2 GetPredictedDirectionToPlayer(Vector2 origin, float missileSpeed, float predictionTimeOffset)
		{
			var playerVel = PlayerSystem.Player.Instance.Velocity;
			float dstToPlayer = origin.DistanceTo(PlayerSystem.Player.Instance.Hitbox.center);
			float timeToPlayer = dstToPlayer / missileSpeed;

			Vector2 predictedPoint = (Vector2)PlayerTransform.position + (playerVel * (timeToPlayer + predictionTimeOffset));
			return origin.DirectionTo(predictedPoint);
		}

		protected virtual void OnDamaged(DamageInstance instance) { }

		protected virtual void OnDeath(DamageInstance instance)
		{
			gameObject.Destroy();
		}

		private void StartRandomSequence()
		{
			if (_sequences.Count == 0)
				throw new System.NullReferenceException($"No sequences registered for AI '{gameObject.name}'!");

			var filtered = _sequences.Where(x => x.CooldownEndTime <= Time.time);
			var sequence = filtered.GetWeightedRandom(x => x.Weight);

			var callback = sequence.Callback();
			PlaySequence(callback);
			sequence.CooldownEndTime = Time.time + sequence.Cooldown;
		}

		private void OrbitPlayer(float minDst, float maxDst)
		{
			float angleOffset = Mathf.Max((Mathf.PerlinNoise1D(Time.time * OrbitFrequency) - 0.5f) * 2f, OrbitFrequencyFloor) * OrbitArc;
			Vector2 dirFromPlayer = Quaternion.AngleAxis(angleOffset, Vector3.forward) * -DirToPlayer;

			float dstFromPlayer = Mathf.Lerp(minDst, maxDst, Mathf.PerlinNoise1D((Time.time + OscillateTimeOffset) * OscillateFrequency));
			Vector2 target = (Vector2)PlayerTransform.position + (dirFromPlayer * dstFromPlayer);

			Movement.MoveTo(target, stoppingDst: 0.01f, setFacingDir: false);
			Movement.SetFacingDirection(DirToPlayer.x);
		}
	}
}
