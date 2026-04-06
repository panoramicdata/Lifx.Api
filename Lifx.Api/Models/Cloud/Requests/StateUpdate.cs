using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the StateUpdate type.
/// </summary>
public class StateUpdate
{
	/// <summary>
	/// Gets or sets Selector.
	/// </summary>
	/// <summary>
	/// Gets or sets Selector.
	/// </summary>
	/// <summary>
	/// Gets or sets Selector.
	/// </summary>
	/// <summary>
	/// Gets or sets Selector.
	/// </summary>
	[JsonPropertyName("selector")]
	public required string Selector { get; set; }

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
	public PowerState? Power { get; set; }

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
	public double? Duration { get; set; }

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
