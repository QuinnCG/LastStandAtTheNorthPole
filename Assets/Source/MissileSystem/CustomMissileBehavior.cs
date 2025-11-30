using UnityEngine;

namespace Quinn.MissileSystem
{
    [System.Serializable]
	public abstract class CustomMissileBehavior
	{
		public virtual void OnSpawn(Missile self) { }
		public virtual void OnHit(GameObject other) { }
		public virtual void OnUpdate() { }
		public virtual void OnDestroy() { }
	}
}
