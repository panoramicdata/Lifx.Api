using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the StateDeltaRequest type.
/// </summary>
public class StateDeltaRequest
{
	/// <summary>
	/// Gets or sets Power.
	/// </summary>
	/// <summary>
	/// Gets or sets Power.
	/// </summary>
	/// <summary>
	/// Gets or sets Power.
	/// </summary>
	/// <summary>
	/// Gets or sets Power.
	/// </summary>
	[JsonPropertyName("power")]
	public string? Power { get; set; }

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
	public double? Hue { get; set; }

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
	public double? Saturation { get; set; }

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
	public double? Kelvin { get; set; }

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
	public double? Brightness { get; set; }

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
	public double? Duration { get; set; } = 1.0;

	/// <summary>
	/// Gets or sets Infrared.
	/// </summary>
	/// <summary>
	/// Gets or sets Infrared.
	/// </summary>
	/// <summary>
	/// Gets or sets Infrared.
	/// </summary>
	/// <summary>
	/// Gets or sets Infrared.
	/// </summary>
	[JsonPropertyName("infrared")]
	public double? Infrared { get; set; }
}
