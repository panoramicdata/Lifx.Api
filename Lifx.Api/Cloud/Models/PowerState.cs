using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Lifx.Api.Cloud.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PowerState
    {
        [EnumMember(Value = "on")]
        On,
        [EnumMember(Value = "off")]
        Off
    }
}
