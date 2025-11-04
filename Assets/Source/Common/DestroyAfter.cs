using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	public class DestroyAfter : MonoBehaviour
	{
		[SerializeField, Unit(Units.Second)]
		private float Delay = 3f;

		private void Awake()
		{
			gameObject.Destroy(Delay);
		}
	}
}
