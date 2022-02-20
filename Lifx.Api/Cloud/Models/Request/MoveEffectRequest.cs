namespace Lifx.Api.Cloud.Models.Request
{
    public class MoveEffectRequest
    {
        public string direction { get; set; } = "forward";
        public double? period { get; set; } = 1;
        public double? cycles { get; set; }
        public bool? power_on { get; set; } = true;
    }
}
