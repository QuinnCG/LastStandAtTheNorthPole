using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	public class GunOrbiter : MonoBehaviour
	{
		public static GunOrbiter Instance { get; private set; }

		[SerializeField, Required]
		private Transform Origin;
		[SerializeField, Required]
		private Transform Handle;
		[SerializeField]
		private float Distance = 0.5f;
		[SerializeField]
		private Ease RecoveryEase = Ease.Linear;

		private float _dst;

		private void Awake()
		{
			Instance = this;
			Handle.SetParent(null);

			_dst = Distance;
		}

		private void OnDestroy()
		{
			if (Handle != null)
			{
				Handle.gameObject.Destroy();
			}
		}

		private void LateUpdate()
		{
			Vector2 origin = Origin.position;
			var target = CrosshairManager.Position;
			Vector2 dir = origin.DirectionTo(target);

			Vector2 pos = origin + (dir * _dst);
			float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

			Handle.SetPositionAndRotation(pos, Quaternion.AngleAxis(rot, Vector3.forward));
			Handle.localScale = new Vector3(1f, Mathf.Sign(dir.x), 1f);
		}

		public void Recoil(float offset, float recoveryTime)
		{
			DOTween.Kill(this);

			_dst = Distance - offset;
			DOTween.To(() => _dst, x => _dst = x, Distance, recoveryTime)
				.SetTarget(this)
				.SetEase(RecoveryEase);
		}
	}
}
