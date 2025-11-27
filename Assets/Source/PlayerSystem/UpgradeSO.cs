using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.PlayerSystem
{
    [CreateAssetMenu]
	public class UpgradeSO : ScriptableObject
	{
		public Sprite Icon;
		[Multiline]
		public string Details;

		[Space, SerializeReference, InlineProperty, HideLabel]
		public Upgrade Upgrade;
	}
}
