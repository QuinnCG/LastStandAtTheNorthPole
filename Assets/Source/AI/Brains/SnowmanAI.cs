using System.Collections;
using UnityEngine;

namespace Quinn.AI.Brains
{
	public class SnowmanAI : AgentAI
	{
        protected override void OnRegisterSequences()
        {
            RegisterSequence(Test);
        }

        private IEnumerator Test()
        {
            Log.Info("Test started!");
            yield return new WaitForSeconds(2f);
            Log.Info("Test ended!");
        }
	}
}
