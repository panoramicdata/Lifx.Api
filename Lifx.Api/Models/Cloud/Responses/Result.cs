using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the Result type.
/// </summary>
public class Result
{
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	/// <summary>
	/// Gets or sets Id.
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	/// <summary>
	/// Gets or sets Label.
	/// </summary>
	/// <summary>
	/// Gets or sets Label.
	/// </summary>
	/// <summary>
	/// Gets or sets Label.
	/// </summary>
	/// <summary>
	/// Gets or sets Label.
	/// </summary>
	[JsonPropertyName("label")]
	public string? Label { get; set; }

	/// <summary>
	/// Gets or sets Status.
	/// </summary>
	/// <summary>
	/// Gets or sets Status.
	/// </summary>
	/// <summary>
	/// Gets or sets Status.
	/// </summary>
	/// <summary>
	/// Gets or sets Status.
	/// </summary>
	[JsonPropertyName("status")]
	public string? Status { get; set; }

	/// <summary>
	/// Gets or sets IsSuccessful.
	/// </summary>
	public bool IsSuccessful { get { return Status == "ok"; } }

	/// <summary>
	/// Gets or sets IsTimedOut.
	/// </summary>
	public bool IsTimedOut { get { return Status == "timed_out"; } }
}
