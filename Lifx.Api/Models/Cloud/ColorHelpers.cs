namespace Lifx.Api.Models.Cloud;

//public abstract class Colors
//{
//    /// <summary>
//    /// Color temperature should be at least 2500K
//    /// </summary>
//    public const int TemperatureMin = 2500;
//    /// <summary>
//    /// Color temperature should be at most 9000K
//    /// </summary>
//    public const int TemperatureMax = 9000;
//    /// <summary>
//    /// A normal white color temperature, this corresponds to the DefaultWhite color.
//    /// </summary>
//    public const int TemperatureDefault = 3500;

//    public const string DefaultWhite = "white";
//    public const string Red = "red";
//    public const string Orange = "orange";
//    public const string Yellow = "yellow";
//    public const string Cyan = "cyan";
//    public const string Green = "green";
//    public const string Blue = "blue";
//    public const string Purple = "purple";
//    public const string Pink = "pink";

//    public static string BuildCustomColor(string saturation, string brightness, string kelvin, string hue)
//    {
//        //check hue
//        if (!IsBetween(Convert.ToInt32(hue), 0, 360))
//        {
//            throw new InvalidConstraintException("Value for Hue is invalid, valid range[0-360]");
//        }

//        //check saturation
//        if (!IsBetween(Convert.ToDouble(saturation), 0.0, 1.0))
//        {
//            throw new InvalidConstraintException("Value for Saturation is invalid, valid range[0.0-1.0]");
//        }

//        //check brightness
//        if (!IsBetween(Convert.ToDouble(brightness), 0.0, 1.0))
//        {
//            throw new InvalidConstraintException("Value for Brightness is invalid, valid range[0.0-1.0]");
//        }

//        //check kelvin
//        if (!IsBetween(Convert.ToDouble(kelvin), 1500, 9000))
//        {
//            throw new InvalidConstraintException("Value for Saturation is invalid, valid range[1500-9000]");
//        }

//        return $"{FormatString("saturation", saturation)} {FormatString("brightness", brightness)} {FormatString("kelvin", kelvin)} {FormatString("hue", hue)}";
//    }

//    public static string BuildRGB(int red, int green, int blue)
//    {
//        //check red
//        if (!IsBetween(Convert.ToDouble(red), 0, 255))
//        {
//            throw new InvalidConstraintException("Value for Red is invalid, valid range[0-255]");
//        }

//        //check green
//        if (!IsBetween(Convert.ToDouble(green), 0, 255))
//        {
//            throw new InvalidConstraintException("Value for Red is invalid, valid range[0-255]");
//        }

//        //check blue
//        if (!IsBetween(Convert.ToDouble(blue), 0, 255))
//        {
//            throw new InvalidConstraintException("Value for Red is invalid, valid range[0-255]");
//        }
//        return $"rgb:{red},{green},{blue}";
//    }

//    private static string FormatString(string element, string value)
//    {
//        if (!string.IsNullOrEmpty(value))
//        {
//            return $"{element}:{value}";
//        }
//        else
//        {
//            return string.Empty;
//        }
//    }

//    private static bool IsBetween(double item, double min, double max)
//    {
//        return item >= min && item <= max;
//    }
//}

///// <summary>
///// A color in its natural Lifx representation
///// </summary>
//[JsonObject(MemberSerialization.OptIn)]
//public class HSBK : Colors
//{
//    [JsonProperty]
//    private float? hue;
//    [JsonProperty]
//    private float? saturation;
//    [JsonProperty]
//    private float? brightness;
//    [JsonProperty]
//    private int? kelvin;

//    public float Hue { get { return hue ?? float.NaN; } }
//    public float Saturation { get { return saturation ?? float.NaN; } }
//    public float Brightness { get { return brightness ?? float.NaN; } }
//    public int Kelvin { get { return kelvin ?? TemperatureDefault; } }
//    internal HSBK() { }
//    public HSBK(float? hue = null, float? saturation = null, float? brightness = null, int? kelvin = null)
//    {
//        if (hue is null && saturation is null && brightness is null && kelvin is null)
//        {
//            throw new ArgumentException("HSBKColor requires at least one non-null component");
//        }
//        this.hue = hue;
//        this.saturation = saturation;
//        this.brightness = brightness;
//        this.kelvin = kelvin;
//    }

//    public override string ToString()
//    {
//        StringBuilder sb = new StringBuilder();
//        if (hue is not null)
//        {
//            sb.AppendFormat("hue:{0} ", Math.Min(Math.Max(0, hue.Value), 360));
//        }
//        if (saturation is not null)
//        {
//            sb.AppendFormat("saturation:{0} ", Math.Min(Math.Max(0, saturation.Value), 1));
//        }
//        if (brightness is not null)
//        {
//            sb.AppendFormat("brightness:{0} ", Math.Min(Math.Max(0, brightness.Value), 1));
//        }
//        if (kelvin is not null && (saturation ?? 0) < 0.001)
//        {
//            sb.AppendFormat("kelvin:{0} ", Math.Min(Math.Max(TemperatureMin, kelvin.Value), TemperatureMax));
//        }
//        sb.Remove(sb.Length - 1, 1);
//        return sb.ToString();
//    }

//    internal HSBK WithBrightness(float brightness)
//    {
//        return new HSBK(this.hue, this.saturation, brightness, this.kelvin);
//    }
//}

///// <summary>
///// A color in its natural Lifx represenation
///// </summary>
//public sealed class HSB : HSBK
//{
//    public HSB(float hue, float saturation = 1f, float brightness = 1f) : base(hue, saturation, brightness) { }
//}

///// <summary>
///// A configurable white Light Color
///// </summary>
//public sealed class White : HSBK
//{
//    public White(float brightness = 1f, int kelvin = TemperatureDefault) : base(null, null, brightness, kelvin) { }
//}

///// <summary>
///// Automatically converted to HSBK by Lifx cloud.
///// </summary>
//public sealed class RGB : Colors
//{
//    public byte R { get; private set; }
//    public byte G { get; private set; }
//    public byte B { get; private set; }

//    public RGB(int r, int g, int b)
//    {
//        R = (byte)Math.Min(Math.Max(r, 0), byte.MaxValue);
//        G = (byte)Math.Min(Math.Max(g, 0), byte.MaxValue);
//        B = (byte)Math.Min(Math.Max(b, 0), byte.MaxValue);
//    }

//    /// <summary>
//    /// Unpack a color from an integer
//    /// </summary>
//    /// <param name="packedRGB">RGB packed integer eg 0xff0000 is bright deep red</param>
//    public RGB(int packedRGB)
//    {
//        R = (byte)((packedRGB >> 16) & 0xff);
//        G = (byte)((packedRGB >> 8) & 0xff);
//        B = (byte)(packedRGB & 0xff);
//    }

//    public override string ToString()
//    {
//        return string.Format("#{0:x2}{1:x2}{2:x2}", R, G, B);
//    }
//}
