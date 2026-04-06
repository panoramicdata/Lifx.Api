using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the EffectsOffRequest type.
/// </summary>
public class EffectsOffRequest
{
	/// <summary>
	/// Gets or sets PowerOff.
	/// </summary>
	/// <summary>
	/// Gets or sets PowerOff.
	/// </summary>
	/// <summary>
	/// Gets or sets PowerOff.
	/// </summary>
	/// <summary>
	/// Gets or sets PowerOff.
	/// </summary>
	[JsonPropertyName("power_off")]
	public bool? PowerOff { get; set; } = false;
}
