using Newtonsoft.Json;

namespace Lifx.Api.Cloud.Models.Response
{
    public class Error
    {
        [JsonProperty("field")]
        public string Field { get; set; }
        [JsonProperty("message")]
        public string[] Message { get; set; }
    }
}
