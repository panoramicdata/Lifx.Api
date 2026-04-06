using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the CycleRequest type.
/// </summary>
public class CycleRequest
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
	public required List<SetStateRequest> States { get; set; }

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
	public required SetStateRequest Defaults { get; set; }

	/// <summary>
	/// Gets or sets Direction.
	/// </summary>
	/// <summary>
	/// Gets or sets Direction.
	/// </summary>
	/// <summary>
	/// Gets or sets Direction.
	/// </summary>
	/// <summary>
	/// Gets or sets Direction.
	/// </summary>
	[JsonPropertyName("direction")]
	public string Direction { get; set; } = "forward";
}
