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

		protected Animator Animator { get; private set; }
		protected Health Health { get; private set; }
		protected CharacterMovement Movement { get; private set; }
		protected Transform Player { get; private set; }

		protected bool HasActiveSequence => _activeSequence != null;
		protected Vector2 DirToPlayer => transform.position.DirectionTo(Player.position);
		protected float DstToPlayer => transform.position.DistanceTo(Player.position);

		private readonly HashSet<AISequence> _sequences = new();

		private Coroutine? _activeRoutine;
		private IEnumerator? _activeSequence;
		private float _idleEndTime;

		protected virtual void Awake()
		{
			Animator = GetComponent<Animator>();
			Health = GetComponent<Health>();
			Movement = GetComponent<CharacterMovement>();

			OnRegisterSequences();
		}

		protected virtual void Start()
		{
			Player = PlayerSystem.Player.Instance.transform;
		}

		protected virtual void Update()
		{
			if (HasActiveSequence)
			{
				OnUpdate(true);
				return;
			}

			if (DstToPlayer < ActRange && Time.time > _idleEndTime)
			{
				StartRandomSequence();
			}
			else
			{
				float angleOffset = Mathf.Max((Mathf.PerlinNoise1D(Time.time * OrbitFrequency) - 0.5f) * 2f, OrbitFrequencyFloor) * OrbitArc;
				Vector2 dirFromPlayer = Quaternion.AngleAxis(angleOffset, Vector3.forward) * -DirToPlayer;

				float dstFromPlayer = Mathf.Lerp(OscillateRange.x, OscillateRange.y, Mathf.PerlinNoise1D((Time.time + OscillateTimeOffset) * OscillateFrequency));
				Vector2 target = (Vector2)Player.position + (dirFromPlayer * dstFromPlayer);

				Movement.MoveTo(target, stoppingDst: 0.01f, setFacingDir: false);
				Movement.SetFacingDirection(DirToPlayer.x);

				OnUpdate(false);
			}
		}

		protected virtual void OnRegisterSequences() { }

		protected virtual void OnUpdate(bool hasActiveSequence) { }

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

		protected void RegisterSequence(System.Func<IEnumerator> callback, float weight = 100f, float cooldown = 0f)
		{
			_sequences.Add(new AISequence(callback, weight, cooldown));
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
	}
}
