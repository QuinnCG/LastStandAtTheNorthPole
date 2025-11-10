using Quinn.DamageSystem;
using Quinn.MovementSystem;
using Unity.Behavior;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(BehaviorGraphAgent))]
	[RequireComponent(typeof(Health))]
	[RequireComponent(typeof(CharacterMovement))]
	public class Agent : MonoBehaviour
	{
		private Health _health;
		private CharacterMovement _movement;

		private void Awake()
		{
			_health = GetComponent<Health>();
			_movement = GetComponent<CharacterMovement>();
		}
	}
}
