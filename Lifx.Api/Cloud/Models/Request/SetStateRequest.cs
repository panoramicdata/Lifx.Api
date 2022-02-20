using Newtonsoft.Json;

namespace Lifx.Api.Cloud.Models.Request
{
    public class SetStateRequest
    {
        [JsonProperty("power")]
        public PowerState Power { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }
        
        [JsonProperty("brightness")]
        public double? Brightness { get; set; }
        [JsonProperty("duration")]
        public double? Duration { get; set; } = 1.0;
        [JsonProperty("infrared")]
        public double? Infrared { get; set; }
        [JsonProperty("fast")]
        public bool? Fast { get; set; } = false;
    }
}
