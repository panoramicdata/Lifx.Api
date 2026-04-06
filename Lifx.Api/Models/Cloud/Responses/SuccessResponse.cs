using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the SuccessResponse type.
/// </summary>
public class SuccessResponse : ApiResponse
{
	/// <summary>
	/// Gets or sets Results.
	/// </summary>
	/// <summary>
	/// Gets or sets Results.
	/// </summary>
	/// <summary>
	/// Gets or sets Results.
	/// </summary>
	/// <summary>
	/// Gets or sets Results.
	/// </summary>
	[JsonPropertyName("results")]
	public List<Result> Results { get; set; } = [];
}
