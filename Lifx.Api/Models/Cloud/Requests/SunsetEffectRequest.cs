using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the SunsetEffectRequest type.
/// </summary>
public class SunsetEffectRequest
{
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
