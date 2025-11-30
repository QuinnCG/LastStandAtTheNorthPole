using FMODUnity;
using Quinn.DamageSystem;
using System;
using System.Collections;
using UnityEngine;

namespace Quinn.AI.Brains
{
    [RequireComponent(typeof(DamageOnContact))]
    public class GingerbreadManAI : AgentAI
	{
		[SerializeField]
		private EventReference FootstepSound, ChargeSound, GiggleSound;
		[SerializeField]
		private float ChargeSpeed = 5f;
		[SerializeField]
		private float ChargeDuration = 3f;
		[SerializeField]
		private float GiggleCooldown = 3f;

		private float _nextGiggleAllowedTime;

        protected override void Awake()
        {
            base.Awake();
			GetComponent<DamageOnContact>().OnContact += OnContact;
        }

        private void OnContact()
        {
            if (Time.time > _nextGiggleAllowedTime)
			{
				_nextGiggleAllowedTime = Time.time + GiggleCooldown;
				Audio.Play(GiggleSound);
			}
        }

        protected override void OnRegisterSequences()
        {
			RegisterSequence(Charge);
        }

		private IEnumerator Charge()
		{
			Audio.Play(ChargeSound);

			Animator.Play("Charging");

			var dir = DirToPlayer;
			float endTime = Time.time + ChargeDuration;

			while (Time.time < endTime)
			{
				Movement.Move(dir, ChargeSpeed);
				yield return null;
			}
		}

		protected override void OnUpdate()
		{
			if (!HasActiveSequence)
			{
				Animator.Play(Movement.IsMoving ? "Moving" : "Idling");
			}
		}

		public void Footstep()
		{
			Audio.Play(FootstepSound, transform.position);
		}
	}
}
