using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.PlayerSystem
{
    [CreateAssetMenu]
	public class UpgradeSO : ScriptableObject
	{
		[Tooltip("The maximum number of types this unique asset can be shown. 0 = infinite.")]
		public int MaxAppearanceCount = 0;

		[Space]

		public Sprite Icon;
		[Multiline(6)]
		public string Details;

		[Space, SerializeReference, InlineProperty, HideLabel]
		public Upgrade Upgrade;
	}
}
