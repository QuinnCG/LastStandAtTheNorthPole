using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Quinn.MissileSystem
{
	public struct MissileMoveJob : IJobParallelFor
	{
		public float DeltaTime;
		public float Time;

		[ReadOnly]
		public NativeArray<MissileBehavior> Behaviors;

		public NativeArray<float2> Directions;
		public NativeArray<bool> ShouldDoHoming;
		public NativeArray<float2> HomingTargets;
		public NativeArray<float2> Positions;

		public void Execute(int index)
		{
			var behavior = Behaviors[index];
			var dir = Directions[index];

			bool shouldDoHoming = ShouldDoHoming[index];

			// HACK: Find a better way to structure this.
			if (shouldDoHoming || behavior.AdditionalTurnRate != 0f)
			{
				// HACK: Consolidate into one call for difference, one call from that for mag, and one more for dir.
				// Maybe just use dir = (float2)(Vector2)(Quaternion.AngleAxis(angleDelta1, Vector3.forward) * (Vector2)dir);
				float currentAngle = Mathf.Atan2(dir.y, dir.x);
				float2 dirToTarget = math.normalize(HomingTargets[index] - Positions[index]);
				float angleToTarget = Mathf.Atan2(dirToTarget.y, dirToTarget.x);

				float dstToTarget = math.distance(HomingTargets[index], Positions[index]);
				float turnRate = Mathf.Lerp(behavior.HomingNearTurnRate, behavior.HomingFarmTurnRate, Mathf.Clamp01(dstToTarget / behavior.MaxHomingDistance));
				turnRate *= Mathf.Deg2Rad;

				float angleDelta = angleToTarget - currentAngle;
				float angleDir = Mathf.Sign(angleDelta);

				float newAngle = currentAngle;

				if (shouldDoHoming)
				{
					newAngle += angleDir * turnRate * DeltaTime;
				}

				newAngle += -behavior.AdditionalTurnRate * Mathf.Deg2Rad * DeltaTime;
				dir = new float2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
			}

			Directions[index] = dir;
			float2 delta = dir * behavior.Speed;

			if (behavior.DoesOscillation)
			{
				var perp = new float2(-delta.y, delta.x);
				delta += perp.LerpTo(-perp, Mathf.Sin(Time * behavior.OscillationFrequency * behavior.Speed).MapTrig01()) * behavior.OscillationAmplitude;
			}

			Positions[index] += delta * DeltaTime;
		}
	}
}
