using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Lifx.Api.Models.Cloud;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PowerState
{
	[EnumMember(Value = "on")]
	On,
	[EnumMember(Value = "off")]
	Off
}
