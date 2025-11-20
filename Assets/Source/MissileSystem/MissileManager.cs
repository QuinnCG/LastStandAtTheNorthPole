using QFSW.QC;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

namespace Quinn.MissileSystem
{
	public class MissileManager : MonoBehaviour
	{
		public static MissileManager Instance { get; private set; }

		[SerializeField]
		private GameObject MissilePrefab;
		[SerializeField]
		private int PoolPrecache = 50;
		[SerializeField]
		private int JobBatchSize = 1;

		private bool _showDebugText;

		private readonly ObjectPool<Missile> _pool = new(CreateObj, EnableObj, DisableObj, DestroyObj);

		private readonly HashSet<(Vector2 pos, Vector2 dir, MissileData data)> _toCreateDeferred = new();
		private readonly List<Missile> _toUpdate = new();
		private readonly HashSet<Missile> _toDestroy = new();

		private void Awake()
		{
			Instance = this;
			_pool.Buffer(PoolPrecache);

			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

        private void OnDestroy()
        {
			SceneManager.sceneUnloaded -= OnSceneUnloaded;
		}

        private void Update()
		{
			int missileCount = _toUpdate.Count;

			var behaviors = new NativeArray<MissileBehavior>(missileCount, Allocator.TempJob);
			var directions = new NativeArray<float2>(missileCount, Allocator.TempJob);
			var shouldDoHoming = new NativeArray<bool>(missileCount, Allocator.TempJob);
			var homingTargets = new NativeArray<float2>(missileCount, Allocator.TempJob);
			var positions = new NativeArray<float2>(missileCount, Allocator.TempJob);

			for (int i = 0; i < missileCount; i++)
			{
				var missile = _toUpdate[i];

				behaviors[i] = missile.Data.Behavior;
				directions[i] = missile.Direction;
				positions[i] = (Vector2)missile.transform.position;

				if (missile.Data.Behavior.DoesHoming && missile.GetHomingTarget(out var pos))
				{
					shouldDoHoming[i] = true;
					homingTargets[i] = pos;
					Debug.DrawLine(missile.transform.position, pos, Color.yellow);
				}
				else
				{
					shouldDoHoming[i] = false;
				}
			}

			var job = new MissileMoveJob()
			{
				DeltaTime = Time.deltaTime,
				Time = Time.time,

				Behaviors = behaviors,
				Directions = directions,
				ShouldDoHoming = shouldDoHoming,
				HomingTargets = homingTargets,
				Positions = positions
			}.Schedule(_toUpdate.Count, JobBatchSize);

			job.Complete();

			for (int i = 0; i < missileCount; i++)
			{
				var missile = _toUpdate[i];

				float2 pos = positions[i];
				missile.transform.position = new(pos.x, pos.y);

				float2 dir = directions[i];
				missile.SetDirection(new(dir.x, dir.y));
			}

			directions.Dispose();
			behaviors.Dispose();
			shouldDoHoming.Dispose();
			homingTargets.Dispose();
			positions.Dispose();
		}

		private void FixedUpdate()
		{
			foreach ((Vector2 pos, Vector2 dir, MissileData data) in _toCreateDeferred)
			{
				var obj = _pool.Create();
				Init(obj, pos, dir, data);
			}

			_toCreateDeferred.Clear();

			foreach (var missile in _toUpdate)
			{
				missile.PhysicsUpdate();
			}

			foreach (var missile in _toDestroy)
			{
				_toUpdate.Remove(missile);
				_pool.Destroy(missile);
			}

			_toDestroy.Clear();
		}

		private void OnGUI()
		{
			if (_showDebugText)
			{
				GUI.color = Color.cyan;
				GUI.Label(new(10f, 0f, 1000f, 100f), $"Missiles: [Active: {_toUpdate.Count}, Dormant: {_pool.Count}]");
			}
		}

		private void OnSceneUnloaded(Scene scene)
		{
			foreach (var missile in _toUpdate)
			{
				_toDestroy.Add(missile);
			}
		}

		/// <summary>
		/// Spawn a missile and initialize it.
		/// </summary>
		/// <param name="pos">The world-space position for the missile to be spawned at.</param>
		/// <param name="dir">The base direction for the missile to move in. The actual direction it moves in may be very different.</param>
		/// <param name="missile">Data about the missile's behavior and visuals.</param>
		/// <param name="defer">If true, the missile will be spawned outside of the missile update loop (potentially on the next frame). You should set this to true when spawning a missile from another missile's PhysicsUpdate().</param>
		public Missile Spawn(Vector2 pos, Vector2 dir, MissileData missile, bool defer = false)
		{
			if (defer)
			{
				_toCreateDeferred.Add((pos, dir, missile));
				return null;
			}

			var instance = _pool.Create();
			Init(instance, pos, dir, missile);

			return instance;
		}

		public Missile[] Spawn(Vector2 pos, Vector2 dir, MissileData missile, MissileSpawnPattern pattern, bool defer = false)
		{
			var spawned = new Missile[pattern.Count];

			for (int i = 0; i < pattern.Count; i++)
			{
				Vector2 newDir = CalculateDir(i, dir, pattern);
				var instance = Spawn(pos, newDir, missile, defer);

				spawned[i] = instance;
			}

			return spawned;
		}

		private Vector2 CalculateDir(int index, Vector2 baseDir, MissileSpawnPattern pattern)
		{
			float halfRange = pattern.ArcSpread / 2f;

			if (pattern.IsRandom)
			{
				float delta;

				if (pattern.SpreadType is MissileSpreadType.Arc)
				{
					delta = UnityEngine.Random.Range(-halfRange, halfRange);
				}
				else
				{
					delta = UnityEngine.Random.Range(0f, 360f);
				}

				return Quaternion.AngleAxis(delta, Vector3.forward) * baseDir;
			}
			else
			{
				if (pattern.Count == 1)
				{
					return Quaternion.AngleAxis(0f, Vector3.forward) * baseDir;
				}

				float deltaPerMissile;
				float delta;

				if (pattern.SpreadType is MissileSpreadType.Arc)
				{
					deltaPerMissile = pattern.ArcSpread / (pattern.Count - 1);
					delta = (deltaPerMissile * index) - halfRange;
				}
				else
				{
					deltaPerMissile = 360f / pattern.Count;
					delta = (deltaPerMissile * index) - (deltaPerMissile);
				}

				return Quaternion.AngleAxis(delta, Vector3.forward) * baseDir;
			}
		}

		/// <summary>
		/// This shouldn't be called directly. Instead call <see cref="Missile.Kill"/>.
		/// </summary>
		public void DestroyMissile(Missile missile)
		{
			_toDestroy.Add(missile);
		}

		private void Init(Missile missile, Vector2 pos, Vector2 dir, MissileData data)
		{
			missile.transform.position = pos;
			missile.Init(dir, data);
			_toUpdate.Add(missile);
		}

		private static Missile CreateObj()
		{
			return Instantiate(Instance.MissilePrefab, Instance.transform).GetComponent<Missile>();
		}

		private static void EnableObj(Missile missile)
		{
			missile.gameObject.SetActive(true);
		}

		private static void DisableObj(Missile missile)
		{
			missile.gameObject.SetActive(false);
		}

		private static void DestroyObj(Missile missile)
		{
			Destroy(missile.gameObject);
		}

		[Command("missile.info", "Toggles a label on the top-left corner of the screen that shows info about alive and pooled missiles.")]
		public void SetMissileDebugInfo(bool? visible = null)
		{
			if (visible.HasValue)
			{
				_showDebugText = visible.Value;
			}
			else
			{
				_showDebugText = !_showDebugText;
			}
		}
	}
}
