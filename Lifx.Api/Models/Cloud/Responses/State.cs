using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the State type.
/// </summary>
public class State
{
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	/// <summary>
	/// Gets or sets Brightness.
	/// </summary>
	[JsonPropertyName("brightness")]
	public required float Brightness { get; init; }

	/// <summary>
	/// Gets or sets Selector.
	/// </summary>
	/// <summary>
	/// Gets or sets Selector.
	/// </summary>
	/// <summary>
	/// Gets or sets Selector.
	/// </summary>
	/// <summary>
	/// Gets or sets Selector.
	/// </summary>
	[JsonPropertyName("selector")]
	public required string Selector { get; init; }

	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	/// <summary>
	/// Gets or sets Color.
	/// </summary>
	[JsonPropertyName("color")]
	public required Hsbk? Color { get; init; }
}
