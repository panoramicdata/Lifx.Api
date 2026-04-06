using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the PulseEffectRequest type.
/// </summary>
public class PulseEffectRequest
{
	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	[JsonPropertyName("color")]
	public required string Color { get; set; }

	/// <summary>
	/// Gets or sets FromColor.
	/// </summary>
	/// <summary>
	/// Gets or sets FromColor.
	/// </summary>
	/// <summary>
	/// Gets or sets FromColor.
	/// </summary>
	/// <summary>
	/// Gets or sets FromColor.
	/// </summary>
	[JsonPropertyName("from_color")]
	public string? FromColor { get; set; }

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
	public double? Period { get; set; } = 1.0;

	/// <summary>
	/// Gets or sets Cycles.
	/// </summary>
	/// <summary>
	/// Gets or sets Cycles.
	/// </summary>
	/// <summary>
	/// Gets or sets Cycles.
	/// </summary>
	/// <summary>
	/// Gets or sets Cycles.
	/// </summary>
	[JsonPropertyName("cycles")]
	public double? Cycles { get; set; } = 1.0;

	/// <summary>
	/// Gets or sets Persist.
	/// </summary>
	/// <summary>
	/// Gets or sets Persist.
	/// </summary>
	/// <summary>
	/// Gets or sets Persist.
	/// </summary>
	/// <summary>
	/// Gets or sets Persist.
	/// </summary>
	[JsonPropertyName("persist")]
	public bool? Persist { get; set; } = false;

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
