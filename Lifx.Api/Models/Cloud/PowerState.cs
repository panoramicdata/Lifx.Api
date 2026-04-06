using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud;

/// <summary>
/// Represents the power state of a device.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PowerState
{
	/// <summary>
	/// Device is powered on.
	/// </summary>
	[EnumMember(Value = "on")]
	On,
	/// <summary>
	/// Device is powered off.
	/// </summary>
	[EnumMember(Value = "off")]
	Off
}
