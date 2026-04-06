using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Requests;

/// <summary>
/// Represents the CleanRequest type.
/// </summary>
public class CleanRequest
{
	/// <summary>
	/// Gets or sets Stop.
	/// </summary>
	/// <summary>
	/// Gets or sets Stop.
	/// </summary>
	/// <summary>
	/// Gets or sets Stop.
	/// </summary>
	/// <summary>
	/// Gets or sets Stop.
	/// </summary>
	[JsonPropertyName("stop")]
	public bool? Stop { get; set; } = false;

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
}
