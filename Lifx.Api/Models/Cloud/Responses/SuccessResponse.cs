using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

public class SuccessResponse : ApiResponse
{
	[JsonPropertyName("results")]
	public List<Result> Results { get; set; } = [];
}
