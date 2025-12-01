using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lifx.Api.Serialization;

/// <summary>
/// JSON converter for dictionaries with string keys and boolean values that may be represented as numbers
/// </summary>
public class CapabilitiesDictionaryConverter : JsonConverter<Dictionary<string, bool>>
{
	private readonly BooleanOrNumberConverter _boolConverter = new();

	public override Dictionary<string, bool>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			return null;
		}

		if (reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException("Expected StartObject token");
		}

		var dictionary = new Dictionary<string, bool>();

		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
			{
				return dictionary;
			}

			if (reader.TokenType != JsonTokenType.PropertyName)
			{
				throw new JsonException("Expected PropertyName token");
			}

			var key = reader.GetString() ?? throw new JsonException("Null property name");

			reader.Read();
			var value = _boolConverter.Read(ref reader, typeof(bool), options);

			dictionary[key] = value;
		}

		throw new JsonException("Unexpected end of JSON");
	}

	public override void Write(Utf8JsonWriter writer, Dictionary<string, bool> value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		foreach (var kvp in value)
		{
			writer.WritePropertyName(kvp.Key);
			_boolConverter.Write(writer, kvp.Value, options);
		}

		writer.WriteEndObject();
	}
}
