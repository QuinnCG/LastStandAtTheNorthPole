using Quinn.PlayerSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Quinn.AI
{
	public class AIManager : MonoBehaviour
	{
		public static AIManager Instance { get; private set; }

		[SerializeField]
		private float MaxSumThreat = 50f;
		[SerializeField, Unit(Units.Second)]
		private float TickInterval = 1f;

		// All live agents.
		private readonly HashSet<AgentAI> _allAgents = new();
		// The total threat score of all engaged agents.
		private float _sumThreatOfEngaged;

		private float _nextTick;

		private void Awake()
		{
			Instance = this;

			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

        private void OnDestroy()
        {
			SceneManager.sceneUnloaded -= OnSceneUnloaded;
		}

        private void OnSceneUnloaded(Scene scene)
        {
			_allAgents.Clear();
			_sumThreatOfEngaged = 0f;
		}

        private void FixedUpdate()
		{
			if (Time.time >= _nextTick)
			{
				_nextTick = Time.time + TickInterval;
				Tick();
			}
		}

		public void Register(AgentAI agent)
		{
			_allAgents.Add(agent);
		}

		public void Unregister(AgentAI agent)
		{
			_allAgents.Remove(agent);

			if (agent.IsEngaged)
			{
				_sumThreatOfEngaged -= agent.Threat;
			}
		}

		private void Tick()
		{
			Vector2 playerPos = Player.Instance.transform.position;

			var disengaged = _allAgents.Where(x => !x.IsEngaged);

			// Disengage agents that wish to.
			foreach (var agent in _allAgents)
			{
				if (!agent.IsEngaged)
					continue;

				if (agent.WantsToDisengage())
				{
					agent.Disengage();
					_sumThreatOfEngaged -= agent.Threat;
				}
			}

			// Engage agents if we have threat budget.
			var closestCandidates = disengaged.OrderBy(x => x.transform.position.DistanceTo(playerPos));

			foreach (var agent in closestCandidates)
			{
				if (_sumThreatOfEngaged >= MaxSumThreat)
					break;

				if (_sumThreatOfEngaged + agent.Threat > MaxSumThreat)
					continue;

				_sumThreatOfEngaged += agent.Threat;
				agent.Engage();
			}
		}
	}
}
