using Newtonsoft.Json;

namespace Lifx.Api.Cloud.Models.Response
{
    public class SuccessResponse : ApiResponse
    {
        [JsonProperty("results")]
        public List<Result> Results { get; set; } = new();
    }
}
