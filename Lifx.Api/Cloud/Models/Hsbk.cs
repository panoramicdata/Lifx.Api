using Newtonsoft.Json;
using System.Text;

namespace Lifx.Api.Cloud.Models
{
    /// <summary>
    /// A color in its natural Lifx representation
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Hsbk
    {
        [JsonProperty]
        private readonly float? hue;
        [JsonProperty]
        private readonly float? saturation;
        [JsonProperty]
        private readonly float? brightness;
        [JsonProperty]
        private readonly int? kelvin;

        public float Hue { get { return hue ?? float.NaN; } }
        public float Saturation { get { return saturation ?? float.NaN; } }
        public float Brightness { get { return brightness ?? float.NaN; } }
        public int Kelvin { get { return kelvin ?? LifxColor.TemperatureDefault; } }
        internal Hsbk() { }
        public Hsbk(float? hue = null, float? saturation = null, float? brightness = null, int? kelvin = null)
        {
            if (hue == null && saturation == null && brightness == null && kelvin == null)
            {
                throw new ArgumentException("HSBKColor requires at least one non-null component");
            }

            this.hue = hue;
            this.saturation = saturation;
            this.brightness = brightness;
            this.kelvin = kelvin;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            if (hue != null)
            {
                sb.AppendFormat("hue:{0} ", Math.Min(Math.Max(0, hue.Value), 360));
            }

            if (saturation != null)
            {
                sb.AppendFormat("saturation:{0} ", Math.Min(Math.Max(0, saturation.Value), 1));
            }

            if (brightness != null)
            {
                sb.AppendFormat("brightness:{0} ", Math.Min(Math.Max(0, brightness.Value), 1));
            }

            if (kelvin != null && (saturation ?? 0) < 0.001)
            {
                sb.AppendFormat("kelvin:{0} ", Math.Min(Math.Max(LifxColor.TemperatureMin, kelvin.Value), LifxColor.TemperatureMax));
            }

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        internal Hsbk WithBrightness(float brightness)
        {
            return new Hsbk(this.hue, this.saturation, brightness, this.kelvin);
        }
    }
}
