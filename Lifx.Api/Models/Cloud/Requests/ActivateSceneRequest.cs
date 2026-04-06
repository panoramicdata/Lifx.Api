using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the ActivateSceneRequest type.
/// </summary>
public class ActivateSceneRequest
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
	public double? Duration { get; set; } = 1.0;

	/// <summary>
	/// Gets or sets Ignore.
	/// </summary>
	/// <summary>
	/// Gets or sets Ignore.
	/// </summary>
	/// <summary>
	/// Gets or sets Ignore.
	/// </summary>
	/// <summary>
	/// Gets or sets Ignore.
	/// </summary>
	[JsonPropertyName("ignore")]
	public List<string> Ignore { get; set; } = [];

	/// <summary>
	/// Gets or sets Overrides.
	/// </summary>
	/// <summary>
	/// Gets or sets Overrides.
	/// </summary>
	/// <summary>
	/// Gets or sets Overrides.
	/// </summary>
	/// <summary>
	/// Gets or sets Overrides.
	/// </summary>
	[JsonPropertyName("overrides")]
	public SetStateRequest Overrides { get; set; } = new();

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
	public bool? Fast { get; set; }
}
