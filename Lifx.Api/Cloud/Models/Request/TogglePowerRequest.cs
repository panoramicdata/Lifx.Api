using Newtonsoft.Json;

namespace Lifx.Api.Cloud.Models.Request
{
    public class TogglePowerRequest
    {
        [JsonProperty("duration")]
        public double? Duration { get; set; } = 1.0;
    }
}
