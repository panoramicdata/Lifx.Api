namespace Lifx.Api.Models.Lan;

/// <summary>
/// Represents the Color value type.
/// </summary>
public struct Color : IEquatable<Color>
{
	/// <summary>
	/// Gets or sets R.
	/// </summary>
	public byte R { get; set; }

	/// <summary>
	/// Gets or sets G.
	/// </summary>
	public byte G { get; set; }

	/// <summary>
	/// Gets or sets B.
	/// </summary>
	public byte B { get; set; }

	/// <summary>
	/// Determines whether this color has the same components as <paramref name="other"/>.
	/// </summary>
	public readonly bool Equals(Color other) => R == other.R && G == other.G && B == other.B;

	/// <inheritdoc />
	public override readonly bool Equals(object? obj) => obj is Color other && Equals(other);

	/// <inheritdoc />
	public override readonly int GetHashCode() => HashCode.Combine(R, G, B);

	/// <summary>
	/// Determines whether two colors have the same components.
	/// </summary>
	public static bool operator ==(Color left, Color right) => left.Equals(right);

	/// <summary>
	/// Determines whether two colors have different components.
	/// </summary>
	public static bool operator !=(Color left, Color right) => !left.Equals(right);
}
