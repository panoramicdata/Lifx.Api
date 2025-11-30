namespace Lifx.Api.Models.Lan;

/// <summary>
/// LIFX Generic Device
/// </summary>
public abstract class Device
{
	internal Device(string hostname, byte[] macAddress, byte service, uint port)
	{
		ArgumentNullException.ThrowIfNull(hostname);

		if (string.IsNullOrWhiteSpace(hostname))
		{
			throw new ArgumentException(nameof(hostname));
		}

		HostName = hostname;
		MacAddress = macAddress;
		Service = service;
		Port = port;
		LastSeen = DateTime.MinValue;
	}

	/// <summary>
	/// Hostname for the device
	/// </summary>
	public string HostName { get; internal set; }

	/// <summary>
	/// Service ID
	/// </summary>
	public byte Service { get; }

	/// <summary>
	/// Service port
	/// </summary>
	public uint Port { get; }

	internal DateTime LastSeen { get; set; }

	/// <summary>
	/// Gets the MAC address
	/// </summary>
	public byte[] MacAddress { get; }

	/// <summary>
	/// Gets the MAC address
	/// </summary>
	public string MacAddressName
	{
		get
		{
			if (MacAddress is null)
			{
				return string.Empty;
			}

			return string.Join(":", MacAddress.Take(6).Select(tb => tb.ToString("X2")).ToArray());
		}
	}
}
