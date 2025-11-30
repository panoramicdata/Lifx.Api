using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Product features and capabilities
/// </summary>
public class ProductFeatures
{
	[JsonPropertyName("hev")]
	[JsonInclude]
	public bool Hev { get; private set; }

	[JsonPropertyName("color")]
	[JsonInclude]
	public bool Color { get; private set; }

	[JsonPropertyName("chain")]
	[JsonInclude]
	public bool Chain { get; private set; }

	[JsonPropertyName("matrix")]
	[JsonInclude]
	public bool Matrix { get; private set; }

	[JsonPropertyName("relays")]
	[JsonInclude]
	public bool Relays { get; private set; }

	[JsonPropertyName("buttons")]
	[JsonInclude]
	public bool Buttons { get; private set; }

	[JsonPropertyName("infrared")]
	[JsonInclude]
	public bool Infrared { get; private set; }

	[JsonPropertyName("multizone")]
	[JsonInclude]
	public bool Multizone { get; private set; }

	[JsonPropertyName("temperature_range")]
	[JsonInclude]
	public int[]? TemperatureRange { get; private set; }

	[JsonPropertyName("extended_multizone")]
	[JsonInclude]
	public bool ExtendedMultizone { get; private set; }

	[JsonPropertyName("min_ext_mz_firmware")]
	[JsonInclude]
	public long? MinExtendedMultizoneFirmware { get; private set; }

	[JsonPropertyName("min_ext_mz_firmware_components")]
	[JsonInclude]
	public int[]? MinExtendedMultizoneFirmwareComponents { get; private set; }
}
