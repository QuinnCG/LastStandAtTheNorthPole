using UnityEngine;

namespace Quinn
{
	public static class GameObjectExtensions
	{
		public static bool IsPlayer(this GameObject gameObject)
		{
			return gameObject.CompareTag("Player");
		}
		public static bool IsPlayer(this Collider2D collider)
		{
			return collider.gameObject.IsPlayer();
		}
		public static bool IsNPC(this GameObject gameObject)
		{
			return gameObject.CompareTag("NPC");
		}
		public static bool IsNPC(this Collider2D collider)
		{
			return collider.gameObject.IsNPC();
		}
		public static bool IsEnemy(this GameObject gameObject)
		{
			return gameObject.CompareTag("Enemy");
		}
		public static bool IsEnemy(this Collider2D collider)
		{
			return collider.gameObject.IsEnemy();
		}
		public static bool IsMissile(this GameObject gameObject)
		{
			return gameObject.CompareTag("Missile");
		}
		public static bool IsMissile(this Collider2D collider)
		{
			return collider.gameObject.IsMissile();
		}
		public static bool IsObstacle(this GameObject gameObject)
		{
			return gameObject.layer == LayerMask.NameToLayer("Obstacle");
		}
		public static bool IsObstacle(this Collider2D collider)
		{
			return collider.gameObject.IsObstacle();
		}

		public static void Destroy(this GameObject prefab)
		{
			Object.Destroy(prefab);
		}
		public static void Destroy(this GameObject prefab, float delay)
		{
			Object.Destroy(prefab, delay);
		}

		public static void Destroy(this Object obj)
		{
			Object.Destroy(obj);
		}

		public static GameObject Clone(this GameObject prefab)
		{
			return Object.Instantiate(prefab);
		}
		public static GameObject Clone(this GameObject prefab, Transform parent)
		{
			return Object.Instantiate(prefab, parent);
		}
		public static GameObject Clone(this GameObject prefab, Vector3 position, Transform parent = default)
		{
			return Object.Instantiate(prefab, position, Quaternion.identity, parent);
		}

		public static T Clone<T>(this GameObject prefab) where T : Component
		{
			return Object.Instantiate(prefab).GetComponent<T>();
		}
		public static T Clone<T>(this GameObject prefab, Transform parent) where T : Component
		{
			return Object.Instantiate(prefab, parent).GetComponent<T>();
		}
		public static T Clone<T>(this GameObject prefab, Vector3 position, Transform parent = default) where T : Component
		{
			return Object.Instantiate(prefab, position, Quaternion.identity, parent).GetComponent<T>();
		}

		/// <summary>
		/// Calls <see cref="Object.DontDestroyOnLoad(Object)"/> on the specified <see cref="GameObject"/> instance.
		/// </summary>
		public static void MakeTransient(this GameObject instance)
		{
			Object.DontDestroyOnLoad(instance);
		}
	}
}
