using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn.MovementSystem
{
	[RequireComponent(typeof(CharacterMovement))]
	public class FootstepVFX : MonoBehaviour
	{
		[SerializeField, Required]
		private VisualEffect VFX;

		private CharacterMovement _movement;

		private void Awake()
		{
			_movement = GetComponent<CharacterMovement>();
		}

		public void Footstep()
		{
			var dir = _movement.LastMoveDirection;
			float angle = Mathf.Atan2(dir.y, dir.x);

			VFX.SetFloat("Angle", angle * Mathf.Rad2Deg);
			VFX.Play();
		}
	}
}
