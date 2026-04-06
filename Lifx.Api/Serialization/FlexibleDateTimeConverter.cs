using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lifx.Api.Serialization;

/// <summary>
/// Represents the FlexibleDateTimeConverter type.
/// </summary>
public class FlexibleDateTimeConverter : JsonConverter<DateTime?>
{
	private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	/// <summary>
	/// Performs Read operation.
	/// </summary>
	public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.TokenType switch
		{
			JsonTokenType.Null => null,
			JsonTokenType.String => ParseDateTimeString(reader.GetString()),
			JsonTokenType.Number => ParseUnixTimestamp(reader.GetDouble()),
			_ => throw new JsonException($"Unable to convert {reader.TokenType} to DateTime")
		};
	}

	private static DateTime? ParseDateTimeString(string? value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}

		if (DateTime.TryParse(value, out var result))
		{
			return DateTime.SpecifyKind(result, DateTimeKind.Utc);
		}

		return null;
	}

	private static DateTime? ParseUnixTimestamp(double timestamp)
	{
		try
		{
			return UnixEpoch.AddSeconds(timestamp);
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// Performs Write operation.
	/// </summary>
	public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
	{
		if (value.HasValue)
		{
			writer.WriteStringValue(value.Value);
		}
		else
		{
			writer.WriteNullValue();
		}
	}
}
