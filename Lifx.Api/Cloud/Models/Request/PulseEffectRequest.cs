namespace Lifx.Api.Cloud.Models.Request
{
    public class PulseEffectRequest
    {
        public string color { get; set; }
        public string from_color { get; set; }
        public double? period { get; set; } = 1.0;
        public double? cycles { get; set; } = 1.0;
        public bool? persist { get; set; } = false;
        public bool? power_on { get; set; } = true;
    }
}
