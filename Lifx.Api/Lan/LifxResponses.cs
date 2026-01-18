namespace Lifx.Api.Lan;

using Lifx.Api.Models.Lan;

/// <summary>
/// Base class for LIFX response types
/// </summary>
public abstract class LifxResponse
{
	internal static LifxResponse Create(
		FrameHeader header,
		MessageType type,
		uint source,
		byte[] payload) => type switch
		{
			MessageType.DeviceAcknowledgement => new AcknowledgementResponse(header, type, payload, source),
			MessageType.DeviceStateLabel => new StateLabelResponse(header, type, payload, source),
			MessageType.LightState => new LightStateResponse(header, type, payload, source),
			MessageType.LightStatePower => new LightPowerResponse(header, type, payload, source),
			MessageType.InfraredState => new InfraredStateRespone(header, type, payload, source),
			MessageType.DeviceStateVersion => new StateVersionResponse(header, type, payload, source),
			MessageType.DeviceStateHostFirmware => new StateHostFirmwareResponse(header, type, payload, source),
			MessageType.DeviceStateService => new StateServiceResponse(header, type, payload, source),
			MessageType.DeviceStateGroup => new LightGroupResponse(header, type, payload, source),
			_ => new UnknownResponse(header, type, payload, source),
		};

	internal LifxResponse(FrameHeader header, MessageType type, byte[] payload, uint source)
	{
		Header = header;
		Type = type;
		Payload = payload;
		Source = source;
	}

	internal FrameHeader Header { get; }
	internal byte[] Payload { get; }
	internal MessageType Type { get; }
	internal uint Source { get; }
}
/// <summary>
/// Response to GetService message.
/// Provides the device Service and port.
/// If the Service is temporarily unavailable, then the port value will be 0.
/// </summary>
internal class StateServiceResponse : LifxResponse
{
	internal StateServiceResponse(FrameHeader header, MessageType type, byte[] payload, uint source) : base(header, type, payload, source)
	{
		Service = payload[0];
		Port = BitConverter.ToUInt32(payload, 1);
	}
	public byte Service { get; }
	public uint Port { get; }
}
/// <summary>
/// Response to GetLabel message. Provides device label.
/// </summary>
internal class StateLabelResponse : LifxResponse
{
	internal StateLabelResponse(FrameHeader header, MessageType type, byte[] payload, uint source) : base(header, type, payload, source)
	{
		if (payload is not null)
		{
			Label = Encoding.UTF8.GetString(payload, 0, payload.Length).Replace("\0", "");
		}
	}
	public string? Label { get; private set; }
}
/// <summary>
/// Sent by a device to provide the current light state
/// </summary>
public class LightStateResponse : LifxResponse
{
	internal LightStateResponse(FrameHeader header, MessageType type, byte[] payload, uint source) : base(header, type, payload, source)
	{
		Hue = BitConverter.ToUInt16(payload, 0);
		Saturation = BitConverter.ToUInt16(payload, 2);
		Brightness = BitConverter.ToUInt16(payload, 4);
		Kelvin = BitConverter.ToUInt16(payload, 6);
		IsOn = BitConverter.ToUInt16(payload, 10) > 0;
		Label = Encoding.UTF8.GetString(payload, 12, 32).Replace("\0", "");
	}
	/// <summary>
	/// Hue
	/// </summary>
	public ushort Hue { get; private set; }
	/// <summary>
	/// Saturation (0=desaturated, 65535 = fully saturated)
	/// </summary>
	public ushort Saturation { get; private set; }
	/// <summary>
	/// Brightness (0=off, 65535=full brightness)
	/// </summary>
	public ushort Brightness { get; private set; }
	/// <summary>
	/// Bulb color temperature
	/// </summary>
	public ushort Kelvin { get; private set; }
	/// <summary>
	/// Power state
	/// </summary>
	public bool IsOn { get; private set; }
	/// <summary>
	/// Light label
	/// </summary>
	public string Label { get; private set; }
}
public class LightGroupResponse : LifxResponse
{
	internal LightGroupResponse(FrameHeader header, MessageType type, byte[] payload, uint source) : base(header, type, payload, source)
	{
		byte[] guidBytes = new byte[16];
		Array.Copy(payload, 0, guidBytes, 0, 16);
		Group = new Guid(guidBytes);
		Label = Encoding.UTF8.GetString(payload, 16, 32).Replace("\0", "");
		UpdatedAt = BitConverter.ToUInt64(payload, 48);
	}
	/// <summary>
	/// Group
	/// </summary>
	public Guid Group { get; private set; }
	/// <summary>
	/// UpdatedAt
	/// </summary>
	public ulong UpdatedAt { get; private set; }
	/// <summary>
	/// Group label
	/// </summary>
	public string Label { get; private set; }
}
internal class LightPowerResponse : LifxResponse
{
	internal LightPowerResponse(FrameHeader header, MessageType type, byte[] payload, uint source) : base(header, type, payload, source)
	{
		IsOn = BitConverter.ToUInt16(payload, 0) > 0;
	}
	public bool IsOn { get; private set; }
}

internal class InfraredStateRespone : LifxResponse
{
	internal InfraredStateRespone(FrameHeader header, MessageType type, byte[] payload, uint source) : base(header, type, payload, source)
	{
		Brightness = BitConverter.ToUInt16(payload, 0);
	}
	public ushort Brightness { get; private set; }
}

/// <summary>
/// Response to GetVersion message.	Provides the hardware version of the device.
/// </summary>
public class StateVersionResponse : LifxResponse
{
	internal StateVersionResponse(FrameHeader header, MessageType type, byte[] payload, uint source) : base(header, type, payload, source)
	{
		Vendor = BitConverter.ToUInt32(payload, 0);
		Product = BitConverter.ToUInt32(payload, 4);
		Version = BitConverter.ToUInt32(payload, 8);
	}
	/// <summary>
	/// Vendor ID
	/// </summary>
	public uint Vendor { get; private set; }
	/// <summary>
	/// Product ID
	/// </summary>
	public uint Product { get; private set; }
	/// <summary>
	/// Hardware version
	/// </summary>
	public uint Version { get; private set; }
}
/// <summary>
/// Response to GetHostFirmware message. Provides host firmware information.
/// </summary>
public class StateHostFirmwareResponse : LifxResponse
{
	internal StateHostFirmwareResponse(FrameHeader header, MessageType type, byte[] payload, uint source) : base(header, type, payload, source)
	{
		var nanoseconds = BitConverter.ToUInt64(payload, 0);
		Build = Utilities.Epoch.AddMilliseconds(nanoseconds * 0.000001);
		//8..15 UInt64 is reserved
		Version = BitConverter.ToUInt32(payload, 16);
	}
	/// <summary>
	/// Firmware build time
	/// </summary>
	public DateTime Build { get; private set; }
	/// <summary>
	/// Firmware version
	/// </summary>
	public uint Version { get; private set; }
}

internal class UnknownResponse : LifxResponse
{
	internal UnknownResponse(FrameHeader header, MessageType type, byte[] payload, uint source) : base(header, type, payload, source) { }
}
