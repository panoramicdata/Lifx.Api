using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the Scene type.
/// </summary>
public class Scene
{
	/// <summary>
	/// Gets or sets Uuid.
	/// </summary>
	/// <summary>
	/// Gets or sets Uuid.
	/// </summary>
	/// <summary>
	/// Gets or sets Uuid.
	/// </summary>
	/// <summary>
	/// Gets or sets Uuid.
	/// </summary>
	[JsonPropertyName("uuid")]
	public required string Uuid { get; init; }

	/// <summary>
	/// Gets or sets Name.
	/// </summary>
	/// <summary>
	/// Gets or sets Name.
	/// </summary>
	/// <summary>
	/// Gets or sets Name.
	/// </summary>
	/// <summary>
	/// Gets or sets Name.
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; init; }

	/// <summary>
	/// Gets or sets Account.
	/// </summary>
	/// <summary>
	/// Gets or sets Account.
	/// </summary>
	/// <summary>
	/// Gets or sets Account.
	/// </summary>
	/// <summary>
	/// Gets or sets Account.
	/// </summary>
	[JsonPropertyName("account")]
	public required Account Account { get; init; }

	/// <summary>
	/// Gets or sets States.
	/// </summary>
	/// <summary>
	/// Gets or sets States.
	/// </summary>
	/// <summary>
	/// Gets or sets States.
	/// </summary>
	/// <summary>
	/// Gets or sets States.
	/// </summary>
	[JsonPropertyName("states")]
	public required List<State> States { get; init; }

	/// <summary>
	/// Gets or sets CreatedAt.
	/// </summary>
	/// <summary>
	/// Gets or sets CreatedAt.
	/// </summary>
	/// <summary>
	/// Gets or sets CreatedAt.
	/// </summary>
	/// <summary>
	/// Gets or sets CreatedAt.
	/// </summary>
	[JsonPropertyName("created_at")]
	public required int CreatedAt { get; init; }

	/// <summary>
	/// Gets or sets UpdatedAt.
	/// </summary>
	/// <summary>
	/// Gets or sets UpdatedAt.
	/// </summary>
	/// <summary>
	/// Gets or sets UpdatedAt.
	/// </summary>
	/// <summary>
	/// Gets or sets UpdatedAt.
	/// </summary>
	[JsonPropertyName("updated_at")]
	public required int UpdatedAt { get; init; }
}
