using UnityEngine;

namespace Quinn
{
	/// <summary>
	/// A static helper class with some useful collision-related properties and methods.
	/// </summary>
	public static class Layers
	{
		/// <summary>
		/// Name of the obstacle layer.
		/// </summary>
		public const string ObstacleName = "Obstacle";
		/// <summary>
		/// Name of the character layer.
		/// </summary>
		public const string FriendlyCharacterName = "FriendlyCharacter";
		public const string HostileCharacterName = "HostileCharacter";

		/// <summary>
		/// Integer mask representing the obstacle layer only.
		/// </summary>
		public static int ObstacleMask { get; } = LayerMask.GetMask(ObstacleName);
		/// <summary>
		/// Integer mask representing the character layer only.
		/// </summary>
		public static int FriendlyCharacterMask { get; } = LayerMask.GetMask(FriendlyCharacterName);
		public static int HostileCharacterMask { get; } = LayerMask.GetMask(HostileCharacterName);
	}
}
