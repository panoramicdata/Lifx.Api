using Newtonsoft.Json;

namespace Lifx.Api.Cloud.Models.Response
{
    public class Result
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        public bool IsSuccessful { get { return Status == "ok"; } }

        public bool IsTimedOut { get { return Status == "timed_out"; } }
    }
}
