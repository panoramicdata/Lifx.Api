using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the ErrorResponse type.
/// </summary>
public class ErrorResponse : ApiResponse
{
	/// <summary>
	/// Gets or sets Error.
	/// </summary>
	/// <summary>
	/// Gets or sets Error.
	/// </summary>
	/// <summary>
	/// Gets or sets Error.
	/// </summary>
	/// <summary>
	/// Gets or sets Error.
	/// </summary>
	[JsonPropertyName("error")]
	public string? Error { get; init; }

	/// <summary>
	/// Gets or sets Errors.
	/// </summary>
	/// <summary>
	/// Gets or sets Errors.
	/// </summary>
	/// <summary>
	/// Gets or sets Errors.
	/// </summary>
	/// <summary>
	/// Gets or sets Errors.
	/// </summary>
	[JsonPropertyName("errors")]
	public List<Error>? Errors { get; init; }
}
