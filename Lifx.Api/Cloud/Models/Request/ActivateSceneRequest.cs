using Newtonsoft.Json;

namespace Lifx.Api.Cloud.Models.Request
{
    public class ActivateSceneRequest
    {
        [JsonProperty("duration")]
        public double? Duration { get; set; } = 1.0;

        [JsonProperty("ignore")]
        public List<string> Ignore { get; set; } = new();

        [JsonProperty("overrides")]
        public SetStateRequest Overrides { get; set; } = new();

        [JsonProperty("fast")]
        public bool? Fast { get; set; }
    }
}
