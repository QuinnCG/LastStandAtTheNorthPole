using Sirenix.OdinInspector;
using System;

namespace Quinn
{
	[Serializable, Toggle(nameof(IsEnabled))]
	public record StringToggle
	{
		public bool IsEnabled;
		public string Value;
	}
}
