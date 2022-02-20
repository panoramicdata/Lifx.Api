namespace Lifx.Api.Cloud.Models.Request
{
    public class CycleRequest
    {
        public List<SetStateRequest> States { get; set; }

        public SetStateRequest Defaults { get; set; }

        public string Direction { get; set; } = "forward";
    }
}
