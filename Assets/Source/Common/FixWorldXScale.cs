using UnityEngine;

namespace Quinn
{
	public class FixWorldXScale : MonoBehaviour
	{
		private void LateUpdate()
		{
			float localX = Mathf.Sign(transform.localScale.x);
			float worldX = Mathf.Sign(transform.lossyScale.x);

			float finalX = 1f;

			if (localX != worldX)
			{
				finalX *= -1f;
			}

			var scale = transform.localScale;
			scale.x = Mathf.Abs(scale.x) * finalX;
			transform.localScale = scale;
		}
	}
}
