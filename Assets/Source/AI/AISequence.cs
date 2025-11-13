using System.Collections;

namespace Quinn.AI
{
    public class AISequence
	{
		public System.Func<IEnumerator> Callback;
		public float Weight;
		public float Cooldown;
		public float CooldownEndTime;

		public AISequence(System.Func<IEnumerator> callback, float weight = 100f, float cooldown = 0f)
		{
			Callback = callback;
			Weight = weight;
			Cooldown = cooldown;
		}
	}
}
