using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the SetStateRequest type.
/// </summary>
public class SetStateRequest
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
	public PowerState Power { get; set; }

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
	public string? Color { get; set; }

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

	/// <summary>
	/// Gets or sets Fast.
	/// </summary>
	/// <summary>
	/// Gets or sets Fast.
	/// </summary>
	/// <summary>
	/// Gets or sets Fast.
	/// </summary>
	/// <summary>
	/// Gets or sets Fast.
	/// </summary>
	[JsonPropertyName("fast")]
	public bool? Fast { get; set; } = false;
}
