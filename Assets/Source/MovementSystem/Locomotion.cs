using UnityEngine;

namespace Quinn.MovementSystem
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Locomotion : MonoBehaviour
	{
		protected Rigidbody2D Rigidbody { get; private set; }

		private Vector2 _cumulativeVel;
		private Vector2 _overrideVel;

		protected virtual void Awake()
		{
			Rigidbody = GetComponent<Rigidbody2D>();
		}

		protected virtual void OnValidate()
		{
			var rb = GetComponent<Rigidbody2D>();
			rb.gravityScale = 0f;
			rb.freezeRotation = true;
			rb.interpolation = RigidbodyInterpolation2D.Interpolate;
		}

		protected virtual void LateUpdate()
		{
			var vel = (_overrideVel.sqrMagnitude > 0f) ? _overrideVel : _cumulativeVel;
			Rigidbody.linearVelocity = vel;

			_cumulativeVel = Vector2.zero;
			_overrideVel = Vector2.zero;
		}

		/// <summary>
		/// Cumulates the velocity for this frame before applying it at the end.
		/// </summary>
		public void AddVelocity(Vector2 velocity)
		{
			_cumulativeVel += velocity;
		}

		/// <summary>
		/// Overrides the velocity for this frame.
		/// </summary>
		public void SetVelocity(Vector2 velocity)
		{
			_overrideVel = velocity;
		}
	}
}
