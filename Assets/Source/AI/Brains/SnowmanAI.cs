using FMODUnity;
using Quinn.MissileSystem;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn.AI.Brains
{
	public class SnowmanAI : AgentAI
	{
		[SerializeField, Required]
		private Transform MissileOrigin;

		[Space]

		[SerializeField, Required]
		private MissileSpawnData LargeSnowball;
		[SerializeField]
		private VisualEffect LargeSnowballFireVFX;
		[SerializeField]
		private EventReference LargeSnowballFireSound;
		[SerializeField]
		private Vector2 PostFireIdle = new(0.2f, 1f);

		[Space]

		[SerializeField, Required]
		private MissileSpawnData SmallSnowball;
		[SerializeField, Required]
		private VisualEffect SmallSnowballFireVFX;
		[SerializeField]
		private EventReference SmallSnowballFireSound;
		[SerializeField]
		private int RapidFireCount = 3;

		protected override void OnRegisterSequences()
		{
			RegisterSequence(RapidFire);
			RegisterSequence(BigShot, 20f);
		}

		protected override void OnUpdate()
		{
            if (!HasActiveSequence)
			{
				Animator.Play(Movement.IsMoving ? "Moving" : "Idling");
			}
		}

		private IEnumerator RapidFire()
		{
			FacePlayer();
			Animator.Play("Idling");
			yield return new WaitForSeconds(0.5f);
			FacePlayer();

			for (int i = 0; i < RapidFireCount; i++)
			{
				Animator.Play("Fire");
				yield return new WaitForSeconds(0.1f);

				FacePlayer();
				SmallSnowballFireVFX.Play();
				Audio.Play(SmallSnowballFireSound, MissileOrigin.position);

				var dir = GetDirectionToPlayer(MissileOrigin.position);

				MissileManager.Instance.Spawn(MissileOrigin.position, dir, SmallSnowball.Missile!, SmallSnowball.Pattern);
				yield return new WaitForSeconds(0.4f);
			}

			yield break;
		}

		private IEnumerator BigShot()
		{
			FacePlayer();
			Animator.Play("Idling");
			yield return new WaitForSeconds(0.5f);
			FacePlayer();

			Animator.Play("Build");
			yield return new WaitForSeconds(0.3f);

			Animator.Play("Fire");
			yield return new WaitForSeconds(0.1f);

			Audio.Play(LargeSnowballFireSound, MissileOrigin.position);
			LargeSnowballFireVFX.Play();

			var dir = GetDirectionToPlayer(MissileOrigin.position);
			MissileManager.Instance.Spawn(MissileOrigin.position, dir, LargeSnowball.Missile!, LargeSnowball.Pattern);

			yield return new WaitForSeconds(0.4f);
			Animator.Play("Idling");
			FacePlayer();
			yield return new WaitForSeconds(PostFireIdle.GetRandom());
		}
	}
}
