using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	/// <summary>
	/// Manages the creation and destruction of objects, avoiding unnecessary instantiations.
	/// </summary>
	/// <typeparam name="T">The type of the object you wish to pool. This can be any <c>class</c> type.</typeparam>
	public class ObjectPool<T> where T : class
	{
		public const int InitQueueSize = 100;

		public int Count => _pool.Count;
		public int MaxPoolSize { get; set; }

		private readonly Func<T> _createObj;
		private readonly Action<T> _enableObj, _disableObj, _destroyObj;

		private readonly Queue<T> _pool;

		/// <param name="createObj">Called to spawn in the object from nothing. This is usually a call to <see cref="UnityEngine.Object.InstantiateAsync()"/>.</param>
		/// <param name="enableObj">Called to enable a given object from a dormant "pooled" state into an active one. This usually just means resetting values and calling <see cref="UnityEngine.GameObject.SetActive(bool)"/>.</param>
		/// <param name="disableObj">Called to disable an object before placing it into the pool. This usually just means called <see cref="UnityEngine.GameObject.SetActive(bool)"/>.</param>
		/// <param name="destroyObj">Called to destroy an object when there is no more room to pool a given object. This usually means cleaning up any resources and calling <see cref="UnityEngine.Object.DestroyObject(UnityEngine.Object)"/>.</param>
		/// <param name="maxPoolSize">This is the maximum number of pooled objects allowed at any given time. Trying to pool an object when this value has been reached will result in the object being destroyed.</param>
		public ObjectPool(Func<T> createObj, Action<T> enableObj, Action<T> disableObj, Action<T> destroyObj, int maxPoolSize = 1024)
		{
			_createObj = createObj;
			_enableObj = enableObj;
			_disableObj = disableObj;
			_destroyObj = destroyObj;

			MaxPoolSize = maxPoolSize;
			_pool = new Queue<T>(InitQueueSize);
		}

		/// <summary>
		/// Spawn in and disable a number of initial objects in the pool.<br/>
		/// You may wish to call this during a loading screen to avoid an initial stutter when spawning in a bunch of objects suddenly.<br/>
		/// Note: you don't need to call this for the object pooling system to work. This is just for potential additional performance gains where applicable.
		/// </summary>
		/// <param name="count">The number of objects to spawn and pool.</param>
		public void Buffer(int count)
		{ 
			if (count > MaxPoolSize)
			{
				Debug.LogWarning("ObjectPool.Buffer(int) has been given a value above the maximum pool size!\n");
			}

			for (int i = 0; i < count; i++)
			{
				var obj = _createObj();
				_disableObj(obj);
				_pool.Enqueue(obj);
			}
		}

		/// <summary>
		/// Retrieves a pooled object for you to use after initializing it. If no object exists, it will construct a new one.
		/// </summary>
		/// <returns>The created or retrieved object.</returns>
		public T Create()
		{
			T obj = _pool.Count > 0 ? _pool.Dequeue() : _createObj();

			_enableObj(obj);
			return obj;
		}

		/// <summary>
		/// Disables and pools the specified object (as if it was destroyed). If there is no more room in the pool, it will actually be destroyed.
		/// </summary>
		/// <param name="obj">The object to try and pool.</param>
		public void Destroy(T obj)
		{
			if (_pool.Count >= MaxPoolSize)
			{
				_disableObj(obj);
				_destroyObj(obj);
				return;
			}

			_disableObj(obj);
			_pool.Enqueue(obj);
		}
	}
}
