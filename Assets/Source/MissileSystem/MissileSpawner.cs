using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.MissileSystem
{
	/// <summary>
	/// Spawns missiles robotically. The spawning is handled internally. If you wish to spawn your own missiles refer to <see cref="MissileSpawner"/>.
	/// </summary>
	public class MissileSpawner : MonoBehaviour
	{
		[SerializeField]
		private float Interval = 0.5f;
		[SerializeField]
		private Vector2 Direction = new(0f, 1f);
		[SerializeField]
		private MissileSpawnPattern Pattern;

		[Space]

		[SerializeField]
		private int MaxSpawn = -1;
		[SerializeField, Tooltip("Useful for profiling.")]
		private bool DebugBreakOnMaxReached;

		[Space]

		[SerializeField]
		private MissileData Missile;

		[ShowInInspector, ReadOnly]
		private int _spawnCount;
		private float _lastSpawnTime;

		private void Update()
		{
			if (Time.time > _lastSpawnTime + Interval)
			{
				if (MaxSpawn > 0 && _spawnCount + 1 > MaxSpawn)
				{
					if (DebugBreakOnMaxReached)
					{
						Debug.Break();
					}

					return;
				}

				MissileManager.Instance.Spawn(transform.position, Direction.normalized, Missile, Pattern);
				_lastSpawnTime = Time.time;
				_spawnCount++;
			}
		}

		private void OnGUI()
		{
			Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
			pos.y = Screen.height - pos.y;

			GUI.Label(new Rect(pos, Vector2.zero), $"{_spawnCount}", new GUIStyle()
			{
				alignment = TextAnchor.MiddleCenter,
				fontStyle = FontStyle.Bold,
				normal =
				{
					textColor = Color.white
				}
			});
		}
	}
}
