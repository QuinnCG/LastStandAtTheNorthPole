using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

namespace Quinn
{
	public class GlobalBootstrap : MonoBehaviour
	{
		public const string GlobalPrefabAddress = "Global.prefab";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void BoostrapAsync()
		{
			var instance = Addressables.InstantiateAsync(GlobalPrefabAddress)
				.WaitForCompletion();

			DontDestroyOnLoad(instance);
		}

#if UNITY_EDITOR
		private void Update()
		{
			if (Keyboard.current.f4Key.wasPressedThisFrame)
			{
				Debug.Break();
			}
		}
#endif
	}
}
