using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn.DamageSystem
{
	[System.Serializable]
	public record ElementData
	{
		[Tooltip("Choose one"), ValidateInput("@IsPowerOfTwo((int)Element)")]
		public Element Element;
		[ChildGameObjectsOnly]
		public VisualEffect VFX;

		protected bool IsPowerOfTwo(int n)
		{
			return (n & (n - 1)) == 0;
		}
	}
}
