using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class RandomSprite : MonoBehaviour
	{
		[SerializeField]
		private Sprite[] Sprites = System.Array.Empty<Sprite>();

		private void Awake()
		{
			if (gameObject.TryGetComponent(out SpriteRenderer renderer))
			{
				renderer.sprite = Sprites.GetRandom();
			}
		}
	}
}
