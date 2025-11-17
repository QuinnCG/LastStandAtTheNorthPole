using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	public class SFXPlayer : MonoBehaviour
	{
		[SerializeField, Unit(Units.Second)]
		private float CooldownPerUniqueEvent;

		private readonly Dictionary<string, float> _pathToCooldownEndTime = new();

		public void SFX(string eventPath)
		{
			if (CooldownPerUniqueEvent > 0f && _pathToCooldownEndTime.ContainsKey(eventPath))
			{
				if (_pathToCooldownEndTime[eventPath] > Time.time)
				{
					return;
				}
				else
				{
					_pathToCooldownEndTime.Remove(eventPath);
				}
			}

			if (CooldownPerUniqueEvent > 0f)
			{
				_pathToCooldownEndTime.Add(eventPath, Time.time + CooldownPerUniqueEvent);
			}

			Audio.Play(eventPath, transform.position);
		}
	}
}
