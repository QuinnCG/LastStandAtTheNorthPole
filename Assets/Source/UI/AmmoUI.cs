using Quinn.PlayerSystem;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class AmmoUI : MonoBehaviour
	{
		[SerializeField, Required]
		private RectTransform ThrobTransform;
		[SerializeField, Required]
		private Image AmmoSprite;
		[SerializeField, Required]
		private TextMeshProUGUI AmmoText;

		[Space]

		[SerializeField, Tooltip("If this is 0.2, then when the ammo is 20% or less than the max, the text will be red and throbbing.")]
		private float ShowRedBelowNormPercentage = 0.2f;
		[SerializeField]
		private Color RedAmmo = Color.red;
		[SerializeField]
		private Vector2 ThrobSize = new(1f, 1.2f);
		[SerializeField]
		private float ThrobFrequency = 1f;

		private Color _defaultColor;

		private void Awake()
		{
			_defaultColor = AmmoText.color;
		}

		private void Update()
		{
			var manager = GunManager.Instance;
			var gun = manager.Equipped;

			bool showRed = false;

			if (gun == null)
			{
				AmmoSprite.sprite = null;
				AmmoSprite.color = new Color(0f, 0f, 0f, 0f);
				AmmoText.text = "0/0";
			}
			else
			{
				AmmoSprite.sprite = gun.AmmoUI;
				AmmoSprite.color = new Color(1f, 1f, 1f, 1f);
				AmmoText.text = $"{manager.Magazine}/{gun.MagazineSize}";

				float normPercent = (float)manager.Magazine / gun.MagazineSize;

				if (normPercent <= ShowRedBelowNormPercentage)
				{
					showRed = true;
				}
			}

			AmmoText.color = showRed ? RedAmmo : _defaultColor;

			float amp = showRed ? 1f : 0f;

			float t = ((Mathf.Sin(Time.time * ThrobFrequency) * amp) + 1f) / 2f;
			float scale = Mathf.Lerp(ThrobSize.x, ThrobSize.y, t);

			ThrobTransform.localScale = Vector3.one * scale;
		}
	}
}
