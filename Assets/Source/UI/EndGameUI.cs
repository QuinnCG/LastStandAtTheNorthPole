using Quinn.WaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Quinn.UI
{
	public class EndGameUI : MonoBehaviour
	{
		[SerializeField]
		private float FadeInTime = 1f, FadeOutTime = 1f;
		[SerializeField]
		private TextMeshProUGUI WavesSurvived;

		private bool _buttonPressed;

		private async void Awake()
		{
			WavesSurvived.text = $"You Made it to Wave {WaveManager.Instance.LastWaveNumber}!";

			InputManager.Instance.RegisterShowCursor(this);
			await TransitionManager.Instance.FadeFromBlackAsync(FadeInTime);
		}

		public async void Retry()
		{
			if (_buttonPressed)
				return;

			_buttonPressed = true;

			InputManager.Instance.UnregisterShowCursor(this);

			await TransitionManager.Instance.FadeToBlackAsync(FadeOutTime);
			await SceneManager.LoadSceneAsync(0);
		}

		public async void Quit()
		{
			if (_buttonPressed)
				return;

			_buttonPressed = true;

			await TransitionManager.Instance.FadeToBlackAsync(FadeOutTime);

#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif

			Application.Quit();
		}
	}
}
