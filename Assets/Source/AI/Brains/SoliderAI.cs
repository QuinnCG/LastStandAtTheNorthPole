using FMODUnity;
using Quinn.MissileSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn.AI.Brains
{
    public class SoliderAI : AgentAI
	{
		[SerializeField]
		private MissileSpawnData MissileSpawn;
		[SerializeField]
		private Transform MissileSpawnPos;
		[SerializeField]
		private VisualEffect FireVFX, SmokeVFX;

		[Space]

		[SerializeField]
		private EventReference FootstepSound, AimSound, FireSound;

        protected override void OnRegisterSequences()
        {
			RegisterSequence(Fire);
        }

		protected override void OnUpdate()
		{
			if (!HasActiveSequence)
			{
				Animator.Play(Movement.IsMoving ? "Moving" : "Idling");
			}
		}

		private IEnumerator Fire()
		{
			Audio.Play(AimSound, MissileSpawnPos.position);
			Animator.Play("Aim");
			FacePlayer();
			yield return new WaitForSeconds(0.93f);

			int maxIndex = Random.Range(0, 3);

			for (int i = 0; i < maxIndex + 1; i++)
			{
				var dir = MissileSpawnPos.position.DirectionTo(PlayerSystem.Player.Instance.Hitbox.center);
				MissileManager.Instance.Spawn(MissileSpawnPos.position, dir, MissileSpawn.Missile!, MissileSpawn.Pattern);
				Audio.Play(FireSound, MissileSpawnPos.position);

				FireVFX.Play();
				Animator.Play("Fire");

				FacePlayer();

				if (i == maxIndex)
				{
					SmokeVFX.Play();
				}

				yield return new WaitForSeconds(0.8f);
			}

			SmokeVFX.Stop();

			FacePlayer();

			Animator.Play("Relax");
			yield return new WaitForSeconds(0.875f);
		}

		public void Footstep()
		{
			Audio.Play(FootstepSound, transform.position);
		}
	}
}
