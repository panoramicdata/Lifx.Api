using Newtonsoft.Json;

namespace Lifx.Api.Cloud.Models.Request
{
    public class BreatheEffectRequest
    {
        [JsonProperty("color")]
        public string Color { get; set; }
        [JsonProperty("from_color")]
        public string FromColor { get; set; }
        [JsonProperty("period")]
        public double? Period { get; set; } = 1.0;
        [JsonProperty("cycles")]
        public double? Cycles { get; set; } = 1.0;
        [JsonProperty("peak")]
        public double? Peak { get; set; } = .5;
        [JsonProperty("persist")]
        public bool? Persist { get; set; } = false;
        [JsonProperty("power_on")]
        public bool? PowerOn { get; set; } = true;
    }
}
