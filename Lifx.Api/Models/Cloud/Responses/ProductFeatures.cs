using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the ProductFeatures type.
/// </summary>
public class ProductFeatures
{
	/// <summary>
	/// Gets or sets Hev.
	/// </summary>
	/// <summary>
	/// Gets or sets Hev.
	/// </summary>
	/// <summary>
	/// Gets or sets Hev.
	/// </summary>
	/// <summary>
	/// Gets or sets Hev.
	/// </summary>
	[JsonPropertyName("hev")]
	[JsonInclude]
	public bool Hev { get; private set; }

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
	[JsonInclude]
	public bool Color { get; private set; }

	/// <summary>
	/// Gets or sets Chain.
	/// </summary>
	/// <summary>
	/// Gets or sets Chain.
	/// </summary>
	/// <summary>
	/// Gets or sets Chain.
	/// </summary>
	/// <summary>
	/// Gets or sets Chain.
	/// </summary>
	[JsonPropertyName("chain")]
	[JsonInclude]
	public bool Chain { get; private set; }

	/// <summary>
	/// Gets or sets Matrix.
	/// </summary>
	/// <summary>
	/// Gets or sets Matrix.
	/// </summary>
	/// <summary>
	/// Gets or sets Matrix.
	/// </summary>
	/// <summary>
	/// Gets or sets Matrix.
	/// </summary>
	[JsonPropertyName("matrix")]
	[JsonInclude]
	public bool Matrix { get; private set; }

	/// <summary>
	/// Gets or sets Relays.
	/// </summary>
	/// <summary>
	/// Gets or sets Relays.
	/// </summary>
	/// <summary>
	/// Gets or sets Relays.
	/// </summary>
	/// <summary>
	/// Gets or sets Relays.
	/// </summary>
	[JsonPropertyName("relays")]
	[JsonInclude]
	public bool Relays { get; private set; }

	/// <summary>
	/// Gets or sets Buttons.
	/// </summary>
	/// <summary>
	/// Gets or sets Buttons.
	/// </summary>
	/// <summary>
	/// Gets or sets Buttons.
	/// </summary>
	/// <summary>
	/// Gets or sets Buttons.
	/// </summary>
	[JsonPropertyName("buttons")]
	[JsonInclude]
	public bool Buttons { get; private set; }

	/// <summary>
	/// Gets or sets Infrared.
	/// </summary>
	/// <summary>
	/// Gets or sets Infrared.
	/// </summary>
	/// <summary>
	/// Gets or sets Infrared.
	/// </summary>
	/// <summary>
	/// Gets or sets Infrared.
	/// </summary>
	[JsonPropertyName("infrared")]
	[JsonInclude]
	public bool Infrared { get; private set; }

	/// <summary>
	/// Gets or sets Multizone.
	/// </summary>
	/// <summary>
	/// Gets or sets Multizone.
	/// </summary>
	/// <summary>
	/// Gets or sets Multizone.
	/// </summary>
	/// <summary>
	/// Gets or sets Multizone.
	/// </summary>
	[JsonPropertyName("multizone")]
	[JsonInclude]
	public bool Multizone { get; private set; }

	/// <summary>
	/// Gets or sets TemperatureRange.
	/// </summary>
	/// <summary>
	/// Gets or sets TemperatureRange.
	/// </summary>
	/// <summary>
	/// Gets or sets TemperatureRange.
	/// </summary>
	/// <summary>
	/// Gets or sets TemperatureRange.
	/// </summary>
	[JsonPropertyName("temperature_range")]
	[JsonInclude]
	public int[]? TemperatureRange { get; private set; }

	/// <summary>
	/// Gets or sets ExtendedMultizone.
	/// </summary>
	/// <summary>
	/// Gets or sets ExtendedMultizone.
	/// </summary>
	/// <summary>
	/// Gets or sets ExtendedMultizone.
	/// </summary>
	/// <summary>
	/// Gets or sets ExtendedMultizone.
	/// </summary>
	[JsonPropertyName("extended_multizone")]
	[JsonInclude]
	public bool ExtendedMultizone { get; private set; }

	/// <summary>
	/// Gets or sets MinExtendedMultizoneFirmware.
	/// </summary>
	/// <summary>
	/// Gets or sets MinExtendedMultizoneFirmware.
	/// </summary>
	/// <summary>
	/// Gets or sets MinExtendedMultizoneFirmware.
	/// </summary>
	/// <summary>
	/// Gets or sets MinExtendedMultizoneFirmware.
	/// </summary>
	[JsonPropertyName("min_ext_mz_firmware")]
	[JsonInclude]
	public long? MinExtendedMultizoneFirmware { get; private set; }

	/// <summary>
	/// Gets or sets MinExtendedMultizoneFirmwareComponents.
	/// </summary>
	/// <summary>
	/// Gets or sets MinExtendedMultizoneFirmwareComponents.
	/// </summary>
	/// <summary>
	/// Gets or sets MinExtendedMultizoneFirmwareComponents.
	/// </summary>
	/// <summary>
	/// Gets or sets MinExtendedMultizoneFirmwareComponents.
	/// </summary>
	[JsonPropertyName("min_ext_mz_firmware_components")]
	[JsonInclude]
	public int[]? MinExtendedMultizoneFirmwareComponents { get; private set; }
}
