using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the MorphEffectRequest type.
/// </summary>
public class MorphEffectRequest
{
	/// <summary>
	/// Gets or sets Period.
	/// </summary>
	/// <summary>
	/// Gets or sets Period.
	/// </summary>
	/// <summary>
	/// Gets or sets Period.
	/// </summary>
	/// <summary>
	/// Gets or sets Period.
	/// </summary>
	[JsonPropertyName("period")]
	public double? Period { get; set; } = 5;

	/// <summary>
	/// Gets or sets Duration.
	/// </summary>
	/// <summary>
	/// Gets or sets Duration.
	/// </summary>
	/// <summary>
	/// Gets or sets Duration.
	/// </summary>
	/// <summary>
	/// Gets or sets Duration.
	/// </summary>
	[JsonPropertyName("duration")]
	public double? Duration { get; set; }

	/// <summary>
	/// Gets or sets PowerOn.
	/// </summary>
	/// <summary>
	/// Gets or sets PowerOn.
	/// </summary>
	/// <summary>
	/// Gets or sets PowerOn.
	/// </summary>
	/// <summary>
	/// Gets or sets PowerOn.
	/// </summary>
	[JsonPropertyName("power_on")]
	public bool? PowerOn { get; set; } = true;

	/// <summary>
	/// Gets or sets Palette.
	/// </summary>
	/// <summary>
	/// Gets or sets Palette.
	/// </summary>
	/// <summary>
	/// Gets or sets Palette.
	/// </summary>
	/// <summary>
	/// Gets or sets Palette.
	/// </summary>
	[JsonPropertyName("palette")]
	public List<string>? Palette { get; set; }
}
