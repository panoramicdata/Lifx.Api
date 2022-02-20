using Newtonsoft.Json;

namespace Lifx.Api.Cloud.Models.Response
{
    public class ErrorResponse : ApiResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("errors")]
        public List<Error> Errors { get; set; }
    }
}
