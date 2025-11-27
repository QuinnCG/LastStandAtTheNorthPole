using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Quinn
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [SerializeField]
        private EventReference Music;

        private bool _isPlaying;
		private EventInstance _music;

        private void Awake()
        {
            Instance = this;
            _music = RuntimeManager.CreateInstance(Music);
		}

        private void OnDestroy()
        {
            Pause();
            _music.release();
		}

        public void Pause()
        {
            if (!_isPlaying)
                return;

            _music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			_isPlaying = false;
		}

        public void Play()
        {
            if (_isPlaying)
                return;

            _music.start();
			_isPlaying = true;
		}
    }
}
