using Quinn.DamageSystem;
using Quinn.MovementSystem;
using System.Collections;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(Health))]
	[RequireComponent(typeof(CharacterMovement))]
	public class AgentAI : MonoBehaviour
	{
		[SerializeField]
		private Vector2 DistanceFromPlayerRange = new(2f, 4f);

		protected Health Health { get; private set; }
		protected CharacterMovement Movement { get; private set; }
		protected Transform Player { get; private set; }

		protected bool HasActiveSequence => _activeSequence != null;
		protected Vector2 DirToPlayer => transform.position.DirectionTo(Player.position);

		private Coroutine _activeRoutine;
		private IEnumerator _activeSequence;

		// TODO: Randomly select a sequence to trigger from a registered (by child class) list of weighted sequences.

		protected virtual void Awake()
		{
			Health = GetComponent<Health>();
			Movement = GetComponent<CharacterMovement>();
		}

		protected virtual void Start()
		{
			Player = PlayerSystem.Player.Instance.transform;
		}

		protected virtual void Update()
		{
			if (_activeSequence != null && _activeSequence.Current == null)
			{
				StopActiveSequence();
			}

			if (!HasActiveSequence)
			{
				float angleOffset = Mathf.Max((Mathf.PerlinNoise1D(Time.time * 0.05f) - 0.5f) * 2f, -0.5f) * 180f;
				Vector2 dirFromPlayer = Quaternion.AngleAxis(angleOffset, Vector3.forward) * -DirToPlayer;

				float dstFromPlayer = Mathf.Lerp(DistanceFromPlayerRange.x, DistanceFromPlayerRange.y, Mathf.PerlinNoise1D((Time.time + 0.5f) * 0.1f));
				Vector2 target = (Vector2)Player.position + (dirFromPlayer * dstFromPlayer);

				Movement.MoveTo(target, stoppingDst: 0.01f, setFacingDir: false);
				Movement.SetFacingDirection(DirToPlayer.x);
			}
		}

		protected void PlaySequence(IEnumerator sequence)
		{
			if (_activeRoutine != null)
			{
				StopActiveSequence();
			}

			_activeSequence = sequence;
			_activeRoutine = StartCoroutine(sequence);
		}

		protected void StopActiveSequence()
		{
			StopCoroutine(_activeRoutine);
			_activeRoutine = null;
			_activeSequence = null;
		}
	}
}
