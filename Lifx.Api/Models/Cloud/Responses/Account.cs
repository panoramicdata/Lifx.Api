using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the Account type.
/// </summary>
public class Account
{
	/// <summary>
	/// Gets or sets UUID.
	/// </summary>
	/// <summary>
	/// Gets or sets UUID.
	/// </summary>
	/// <summary>
	/// Gets or sets UUID.
	/// </summary>
	/// <summary>
	/// Gets or sets UUID.
	/// </summary>
	[JsonPropertyName("uuid")]
	public required string UUID { get; init; }
}
