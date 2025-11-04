using System.Collections.Generic;
using UnityEngine;

namespace Quinn.DamageSystem
{
	[RequireComponent(typeof(Health))]
	public class StatusManager : MonoBehaviour
	{
		[SerializeField]
		private ElementData[] ElementData;

		private readonly Dictionary<Element, ElementData> _data = new();

		private void Awake()
		{
			var health = GetComponent<Health>();
			health.OnDamage += OnDamaged;

			foreach (var data in ElementData)
			{
				_data.Add(data.Element, data);
			}
		}

		private void OnDamaged(DamageInstance instance)
		{
			var elements = GetIndividualElements(instance.Info.Element);

			foreach (var element in elements)
			{
				if (_data.TryGetValue(element, out var data))
				{
					data.VFX.Play();
				}
			}
		}

		private IEnumerable<Element> GetIndividualElements(Element flags)
		{
			var set = new HashSet<Element>();
			
			foreach (var element in System.Enum.GetValues(typeof(Element)))
			{
				var e = (Element)element;

				if (HasElements(flags, e))
				{
					set.Add(e);
				}
			}

			return set;
		}

		private bool HasElements(Element flags, Element mask)
		{
			return (flags & mask) != 0;
		}
	}
}
