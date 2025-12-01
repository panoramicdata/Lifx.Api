using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lifx.Api.Serialization;

/// <summary>
/// JSON converter that handles boolean values that may be represented as numbers (0/1) in JSON
/// </summary>
public class BooleanOrNumberConverter : JsonConverter<bool>
{
	public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.TokenType switch
		{
			JsonTokenType.True => true,
			JsonTokenType.False => false,
			JsonTokenType.Number => reader.GetInt32() != 0,
			JsonTokenType.String => bool.TryParse(reader.GetString(), out var result) && result,
			_ => throw new JsonException($"Unable to convert {reader.TokenType} to Boolean")
		};
	}

	public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
	{
		writer.WriteBooleanValue(value);
	}
}
