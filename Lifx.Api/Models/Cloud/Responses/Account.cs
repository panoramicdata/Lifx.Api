using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

public class Account
{
	[JsonPropertyName("uuid")]
	public required string UUID { get; init; }
}
