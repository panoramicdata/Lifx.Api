using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

public class Scene
{
	[JsonPropertyName("uuid")]
	public required string UUID { get; init; }

	[JsonPropertyName("name")]
	public required string Name { get; init; }

	[JsonPropertyName("account")]
	public required Account Account { get; init; }

	[JsonPropertyName("states")]
	public required List<State> States { get; init; }

	[JsonPropertyName("created_at")]
	public required int CreatedAt { get; init; }

	[JsonPropertyName("updated_at")]
	public required int UpdatedAt { get; init; }
}

public class State
{
	[JsonPropertyName("brightness")]
	public required float Brightness { get; init; }

	[JsonPropertyName("selector")]
	public required string Selector { get; init; }

	[JsonPropertyName("color")]
	public required Hsbk? Color { get; init; }
}
