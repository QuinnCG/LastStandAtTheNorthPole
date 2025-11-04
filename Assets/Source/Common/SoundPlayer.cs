using FMODUnity;
using UnityEngine;

namespace Quinn
{
	public class SoundPlayer : MonoBehaviour
	{
		public void Play(string path)
		{
			RuntimeManager.PlayOneShot(path, transform.position);
		}
	}
}
