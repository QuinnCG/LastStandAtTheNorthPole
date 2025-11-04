using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Quinn
{
	[RequireComponent(typeof(PlayerInput))]
	public class InputManager : MonoBehaviour
	{
		public const string MoveInput = "Move";
		public const string DashInput = "Dash";
		public const string HealInput = "Heal";
		public const string PrimaryCastInput = "PrimaryCast";
		public const string SecondaryCastInput = "SecondaryCast";
		public const string GamepadAimInput = "Aim";

		public static InputManager Instance { get; private set; }

		public static bool IsInputDisabled => _inputBlockers.Count > 0 && !IsUsingGamepad;
		public static bool IsCursorVisible => _forceShowCursorInstances.Count > 0;
		public static bool IsUsingGamepad { get; private set; }

		private static readonly HashSet<object> _forceShowCursorInstances = new();
		private static readonly HashSet<object> _inputBlockers = new();

		/// <summary>
		/// This value is normalized by the input action in its global input mapping asset.
		/// </summary>
		public Vector2 MoveInputDir { get; private set; }

		public event System.Action<Vector2> OnGamepadAim;
		public event System.Action OnMoveStart, OnMoveStop;
		public event System.Action OnDash;
		public event System.Action OnHeal;
		public event System.Action OnPrimaryCastStart, OnPrimaryCastStop;
		public event System.Action OnSecondaryCastStart, OnSecondaryCastStop;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void StaticReset()
		{
			_forceShowCursorInstances.Clear();
			_inputBlockers.Clear();
		}

		private void Awake()
		{
			Instance = this;

			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Confined;

			var input = GetComponent<PlayerInput>();

			input.onControlsChanged += OnControlsChanged;
			input.onActionTriggered += OnActionTriggered;
		}

		private void Update()
		{
			Cursor.visible = IsCursorVisible;

			if (IsInputDisabled)
			{
				MoveInputDir = Vector2.zero;
			}
		}

		private void OnDestroy()
		{
			Instance = null;
		}

		public void RegisterShowCursor<T>(T key) where T : class
		{
			if (!_forceShowCursorInstances.Add(key))
			{
				Log.Error("Force show cursor key is already in use!");
			}
		}

		public void UnregisterShowCursor<T>(T key) where T : class
		{
			_forceShowCursorInstances.Remove(key);
		}

		/// <summary>
		/// Disable all input routed through <see cref="InputManager"/>.<br/>
		/// Make sure you have a reference to the key you use to that you can re-enable input later via <see cref="UnblockInput(object)"/>.
		/// </summary>
		/// <param name="key">A reference to be used as a key.</param>
		public void BlockInput<T>(T key) where T : class
		{
			if (!_inputBlockers.Add(key))
			{
				Log.Error("Input blocker key is already in use!");
			}
		}

		/// <summary>
		/// Remove a key blocking all input from working.<br/>
		/// This doesn't mean input will actually be enabled again, just that this key won't be used in blocking said input.
		/// </summary>
		/// <param name="key">The reference used to block gameplay input via <see cref="BlockInput(object)"/></param>.

		public void UnblockInput<T>(T key) where T : class
		{
			_inputBlockers.Remove(key);
		}

		/* PLAYER INPUT EVENTS */

		private void OnControlsChanged(PlayerInput input)
		{
			if (input.devices.Count == 0)
			{
				IsUsingGamepad = false;
				return;
			}

			var device = input.devices[0];

			if (device is Gamepad)
			{
				IsUsingGamepad = true;
			}
			else if (device is Keyboard or Mouse)
			{
				IsUsingGamepad = false;
			}
		}

		private void OnActionTriggered(InputAction.CallbackContext context)
		{
			if (IsInputDisabled)
				return;

			switch (context.action.name)
			{
				case MoveInput:
				{
					if (context.performed)
					{
						MoveInputDir = context.action.ReadValue<Vector2>();
						OnMoveStart?.Invoke();
					}
					else if (context.canceled)
					{
						MoveInputDir = Vector2.zero;
						OnMoveStop?.Invoke();
					}

					break;
				}
				case DashInput:
				{
					OnDash?.Invoke();
					break;
				}
				case HealInput:
				{
					OnHeal?.Invoke();
					break;
				}
				case PrimaryCastInput:
				{
					if (context.performed)
					{
						OnPrimaryCastStart?.Invoke();
					}
					else if (context.canceled)
					{
						OnPrimaryCastStop?.Invoke();
					}

					break;
				}
				case SecondaryCastInput:
				{
					if (context.performed)
					{
						OnSecondaryCastStart?.Invoke();
					}
					else if (context.canceled)
					{
						OnSecondaryCastStop?.Invoke();
					}

					break;
				}
				case GamepadAimInput:
				{
					OnGamepadAim?.Invoke(context.action.ReadValue<Vector2>().normalized);
					break;
				}
			}
		}
	}
}
