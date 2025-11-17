using QFSW.QC;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Quinn.DamageSystem
{
	[RequireComponent(typeof(Health))]
	public class StatusManager : MonoBehaviour
	{
		/// <summary>
		/// The number of unique status effects currently applied.
		/// </summary>
		public int StatusCount => _statusEffectEndTimes.Count;
		private readonly Dictionary<StatusEffect, float> _statusEffectEndTimes = new();

		private void Awake()
		{
			var health = GetComponent<Health>();
			health.OnDamaged += OnDamaged;
		}

		private void FixedUpdate()
		{
			var toRemove = new HashSet<StatusEffect>();

			foreach (var entry in _statusEffectEndTimes)
			{
				if (Time.time > entry.Value)
				{
					toRemove.Add(entry.Key);
				}
			}

			foreach (var status in toRemove)
			{
				_statusEffectEndTimes.Remove(status);
			}
		}

		public bool HasStatus(StatusEffect status)
		{
			return _statusEffectEndTimes.ContainsKey(status);
		}

		public float GetStatusDuration(StatusEffect status)
		{
			if (HasStatus(status))
			{
				return Mathf.Max(0f, _statusEffectEndTimes[status] - Time.time);
			}

			return 0f;
		}

		public bool TryGetStatus(StatusEffect status, out float duration)
		{
			if (HasStatus(status))
			{
				duration = _statusEffectEndTimes[status];
				return true;
			}

			duration = 0f;
			return false;
		}

		public void ApplyStatus(StatusEffect status, float duration)
		{
			if (_statusEffectEndTimes.ContainsKey(status))
			{
				_statusEffectEndTimes[status] += duration;
			}
			else
			{
				_statusEffectEndTimes.Add(status, duration);
			}
		}

		public bool RemoveStatus(StatusEffect status)
		{
			return _statusEffectEndTimes.Remove(status);
		}

		public void SetStatusDuration(StatusEffect status, float duration)
		{
			if (!HasStatus(status))
			{
				ApplyStatus(status, duration);
			}
			else
			{
				_statusEffectEndTimes[status] = duration;
			}
		}

		private void OnDamaged(DamageInstance instance)
		{
			foreach (var statusTimePair in instance.Info.StatusEffects)
			{
				ApplyStatus(statusTimePair.Status, statusTimePair.Duration);
			}
		}

		[Command("status.list")]
		protected void ListStatuses_Cmd()
		{
			var builder = new StringBuilder();

			if (_statusEffectEndTimes.Count == 0)
			{
				builder.AppendLine("  - No status effects on player.");
			}
			else
			{
				foreach (var entry in _statusEffectEndTimes)
				{
					builder.AppendLine($"  - {entry.Key}: {entry.Value}s");
				}
			}

			Log.Notice(builder.ToString());
		}

		[Command("status.apply")]
		protected void ApplyStatus_Cmd(StatusEffect status, float duration = 5f)
		{
			ApplyStatus(status, duration);
		}

		[Command("status.remove")]
		protected void RemoveStatus_Cmd(StatusEffect status)
		{
			RemoveStatus(status);
		}
	}
}
