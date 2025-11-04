using UnityEngine;

namespace Quinn
{
	public class SFXPlayer : MonoBehaviour
	{
		public void Play(string eventPath)
		{
			Audio.Play(eventPath, transform.position);
		}
	}
}
