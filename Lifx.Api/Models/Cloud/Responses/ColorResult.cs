using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the ColorResult type.
/// </summary>
public class ColorResult
{
	/// <summary>
	/// Gets or sets Hue.
	/// </summary>
	/// <summary>
	/// Gets or sets Hue.
	/// </summary>
	/// <summary>
	/// Gets or sets Hue.
	/// </summary>
	/// <summary>
	/// Gets or sets Hue.
	/// </summary>
	[JsonPropertyName("hue")]
	public int? Hue { get; set; }

	/// <summary>
	/// Gets or sets Saturation.
	/// </summary>
	/// <summary>
	/// Gets or sets Saturation.
	/// </summary>
	/// <summary>
	/// Gets or sets Saturation.
	/// </summary>
	/// <summary>
	/// Gets or sets Saturation.
	/// </summary>
	[JsonPropertyName("saturation")]
	public float? Saturation { get; set; }

	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	[JsonPropertyName("brightness")]
	public float? Brightness { get; set; }

	/// <summary>
	/// Gets or sets Kelvin.
	/// </summary>
	/// <summary>
	/// Gets or sets Kelvin.
	/// </summary>
	/// <summary>
	/// Gets or sets Kelvin.
	/// </summary>
	/// <summary>
	/// Gets or sets Kelvin.
	/// </summary>
	[JsonPropertyName("kelvin")]
	public float? Kelvin { get; set; }
}
