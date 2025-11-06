using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Quinn.PlayerSystem
{
	public class CrosshairManager : MonoBehaviour
	{
		public static Vector2 CrosshairPos { get; private set; }

		[SerializeField, Required]
		private Transform Crosshair;

		private void Awake()
		{
			Crosshair.SetParent(null);
		}

		private void OnDestroy()
		{
			if (Crosshair != null)
			{
				Crosshair.gameObject.Destroy();
			}
		}

		private void LateUpdate()
		{
			var mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
			mouseWorldPos.z = 0f;

			Crosshair.position = mouseWorldPos;
			CrosshairPos = mouseWorldPos;
		}
	}
}
