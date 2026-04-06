using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the FlameEffectRequest type.
/// </summary>
public class FlameEffectRequest
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
}
