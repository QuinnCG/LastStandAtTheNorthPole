using Quinn.WaveSystem;
using UnityEngine;

namespace Quinn.UI
{
	public class LastEnemyIndicatorUI : MonoBehaviour
	{
		[SerializeField]
		private float EnemyMinDstToShow = 5f;
		[SerializeField]
		private float ArrowMaxDstFromCamCenter = 3f;

		private void LateUpdate()
		{
			var enemyPos = WaveManager.Instance.LastEnemyPos;
			var cam = Camera.main;

			int count = WaveManager.Instance.AliveInCurrentWave;
			gameObject.SetActive(count is 1 && enemyPos.DistanceTo(cam.transform.position) >= EnemyMinDstToShow);

			var diff = enemyPos - (Vector2)cam.transform.position;
			var dir = diff.normalized;
			float mag = Mathf.Min(diff.magnitude, ArrowMaxDstFromCamCenter);

			var newPos = (Vector2)cam.transform.position + (dir * mag);
			transform.position = newPos;

			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}
}
