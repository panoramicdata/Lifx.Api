namespace Lifx.Api.Models.Lan;

/// <summary>
/// Represents the Device type.
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
	/// Gets or sets HostName.
	/// </summary>
	public string HostName { get; internal set; }

	/// <summary>
	/// Gets or sets Service.
	/// </summary>
	public byte Service { get; }

	/// <summary>
	/// Gets or sets Port.
	/// </summary>
	public uint Port { get; }

	internal DateTime LastSeen { get; set; }

	/// <summary>
	/// Gets or sets MacAddress.
	/// </summary>
	public byte[] MacAddress { get; }

	/// <summary>
	/// Represents a public API member.
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
