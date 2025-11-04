using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.MissileSystem
{
	/// <summary>
	/// Contains only blittable data types that will be used in a jobs instance to update the position of the missile.
	/// </summary>
	[System.Serializable]
	public struct MissileBehavior
	{
		public float Speed;

		[Space]

		public bool DoesOscillation;
		public float OscillationAmplitude;
		public float OscillationFrequency;

		[Space]

		public bool DoesHoming;
		public float MaxHomingDistance;
		[Unit(Units.DegreesPerSecond)]
		public float HomingNearTurnRate;
		[Unit(Units.DegreesPerSecond)]
		public float HomingFarmTurnRate;

		[Space]

		[Unit(Units.DegreesPerSecond)]
		public float AdditionalTurnRate;
	}
}
