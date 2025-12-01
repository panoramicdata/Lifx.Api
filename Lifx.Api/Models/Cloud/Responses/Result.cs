using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

public class Result
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("label")]
	public string? Label { get; set; }

	[JsonPropertyName("status")]
	public string? Status { get; set; }

	public bool IsSuccessful { get { return Status == "ok"; } }

	public bool IsTimedOut { get { return Status == "timed_out"; } }
}
