using System.Data;
using System.Text;

namespace Lifx.Api.Cloud.Models
{
    /// <summary>
    /// The color of light is best represented in terms of hue, saturation, 
    /// kelvin, and brightness components. However, other means of expressing
    /// colors are available
    /// </summary>
    public abstract class LifxColor
    {
        /// <summary>
        /// Color temperature should be at least 2500K
        /// </summary>
        public const int TemperatureMin = 1500;
        /// <summary>
        /// Color temperature should be at most 9000K
        /// </summary>
        public const int TemperatureMax = 9000;
        /// <summary>
        /// A normal white color temperature, this corresponds to the DefaultWhite color.
        /// </summary>
        public const int TemperatureDefault = 3500;
        /// <summary>
        /// Sets saturation to 0
        /// </summary>
        public static readonly string DefaultWhite = "white";
        /// <summary>
        /// Sets hue to 0
        /// </summary>
        public static readonly string Red = "red";
        /// <summary>
        /// Sets hue to 34
        /// </summary>
        public static readonly string Orange = "orange";
        /// <summary>
        /// Sets hue to 60
        /// </summary>
        public static readonly string Yellow = "yellow";
        /// <summary>
        /// Sets hue to 180
        /// </summary>
        public static readonly string Cyan = "cyan";
        /// <summary>
        /// Sets hue to 120
        /// </summary>
        public static readonly string Green = "green";
        /// <summary>
        /// Sets hue to 250
        /// </summary>
        public static readonly string Blue = "blue";
        /// <summary>
        /// Sets hue to 280
        /// </summary>
        public static readonly string Purple = "purple";
        /// <summary>
        /// Sets hue to 325
        /// </summary>
        public static readonly string Pink = "pink";

        /// <summary>
        /// A configurable white Light Color
        /// </summary>
        public static string White = BuildHSBK(null, null, 1f, TemperatureDefault);


        public static readonly IEnumerable<string> NamedColors = new List<string>()
        {
            DefaultWhite, Red, Orange, Yellow, Cyan, Green, Blue, Purple, Pink
        };

        public override bool Equals(object obj)
        {
            return obj is LifxColor color && color.ToString() == ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static string BuildHSBK(double? hue, double? saturation, double? brightness, int? kelvin)
        {
            if (hue == null && saturation == null && brightness == null && kelvin == null)
            {
                throw new ArgumentException("HSBKColor requires at least one non-null component");
            }

            string color = BuildHSB(hue, saturation, brightness);

            //check kelvin
            if (kelvin != null && !IsBetween(kelvin.Value, TemperatureMin, TemperatureMax))
            {
                throw new InvalidConstraintException("Value for Saturation is invalid, valid range[1500-9000]");
            }
            else
            {
                color += $" {FormatString("kelvin", kelvin.ToString())}";
            }

            return color;
        }
        public static string BuildHSB(double? hue, double? saturation, double? brightness)
        {
            if (hue == null && saturation == null && brightness == null)
            {
                throw new ArgumentException("HSBColor requires at least one non-null component");
            }

            StringBuilder colorString = new();
            //check hue
            if (hue != null && !IsBetween(hue.Value, 0, 360))
            {
                throw new InvalidConstraintException("Value for Hue is invalid, valid range[0-360]");
            }
            else
            {
                colorString.Append(FormatString(" hue", hue.ToString()));
            }

            //check saturation
            if (saturation != null && !IsBetween(saturation.Value, 0.0, 1.0))
            {
                throw new InvalidConstraintException("Value for Saturation is invalid, valid range[0.0-1.0]");
            }
            else
            {
                colorString.Append(FormatString(" saturation", saturation.ToString()));
            }

            //check brightness
            if (brightness != null && !IsBetween(brightness.Value, 0.0, 1.0))
            {
                throw new InvalidConstraintException("Value for Brightness is invalid, valid range[0.0-1.0]");
            }
            else
            {
                colorString.Append(FormatString(" brightness", brightness.ToString()));
            }

            return colorString.ToString();
        }

        public static string BuildRGB(int red, int green, int blue)
        {
            //check red
            if (!IsBetween(Convert.ToDouble(red), 0, 255))
            {
                throw new InvalidConstraintException("Value for Red is invalid, valid range[0-255]");
            }

            //check green
            if (!IsBetween(Convert.ToDouble(green), 0, 255))
            {
                throw new InvalidConstraintException("Value for Red is invalid, valid range[0-255]");
            }

            //check blue
            if (!IsBetween(Convert.ToDouble(blue), 0, 255))
            {
                throw new InvalidConstraintException("Value for Red is invalid, valid range[0-255]");
            }

            return $"rgb:{red},{green},{blue}";
        }

        private static string FormatString(string element, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return $"{element}:{value}";
            }
            else
            {
                return string.Empty;
            }
        }

        private static bool IsBetween(double item, double min, double max)
        {
            return item >= min && item <= max;
        }
    }
}
