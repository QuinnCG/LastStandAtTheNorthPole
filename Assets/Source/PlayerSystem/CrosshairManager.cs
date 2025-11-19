using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Quinn.PlayerSystem
{
	public class CrosshairManager : MonoBehaviour
	{
		public static Vector2 Position { get; private set; }
		public static Vector2 Direction { get; private set; }

		[SerializeField, Required]
		private Transform Origin;
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
			Position = mouseWorldPos;

			Direction = Origin.position.DirectionTo(Position);
		}

		public void HideCrosshair()
		{
			Crosshair.gameObject.SetActive(false);
		}
	}
}
