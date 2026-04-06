using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the SetStatesRequest type.
/// </summary>
public class SetStatesRequest
{
	/// <summary>
	/// Gets or sets States.
	/// </summary>
	/// <summary>
	/// Gets or sets States.
	/// </summary>
	/// <summary>
	/// Gets or sets States.
	/// </summary>
	/// <summary>
	/// Gets or sets States.
	/// </summary>
	[JsonPropertyName("states")]
	public required List<StateUpdate> States { get; set; }

	/// <summary>
	/// Gets or sets Defaults.
	/// </summary>
	/// <summary>
	/// Gets or sets Defaults.
	/// </summary>
	/// <summary>
	/// Gets or sets Defaults.
	/// </summary>
	/// <summary>
	/// Gets or sets Defaults.
	/// </summary>
	[JsonPropertyName("defaults")]
	public required StateDefaults Defaults { get; set; }
}

/// <summary>
/// Represents the StateDefaults type.
/// </summary>
public class StateDefaults
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
	public PowerState? Power { get; set; }

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
