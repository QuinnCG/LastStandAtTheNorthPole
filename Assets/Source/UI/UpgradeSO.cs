using Quinn.PlayerSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.UI
{
    [CreateAssetMenu]
	public class UpgradeSO : ScriptableObject
	{
		public UpgradeType Type;
		public Sprite Icon;
		[Multiline]
		public string Details;

		[Space, ShowIf(nameof(Type), UpgradeType.Weapon), AssetsOnly]
		public Gun Gun;
		
		// TODO: How to handle other upgrade types?
	}
}
