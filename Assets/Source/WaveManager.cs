using QFSW.QC;
using Quinn.UI;
using UnityEngine;

namespace Quinn
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }

        public int WaveNumber { get; private set; } = 0;

		private void Awake()
        {
            Instance = this;
		}

        [Command("wave.next")]
        public void StartNextWave()
        {
            WaveNumber++;
            WaveManagerUI.Instance.PlayNewWaveSequence();
		}
	}
}
