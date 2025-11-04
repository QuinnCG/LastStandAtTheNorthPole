using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Collider2D))]
	public class Trigger : MonoBehaviour
	{
		public event System.Action OnEnter, OnExit;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.IsPlayer())
			{
				OnEnter?.Invoke();
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.IsPlayer())
			{
				OnExit?.Invoke();
			}
		}
	}
}
