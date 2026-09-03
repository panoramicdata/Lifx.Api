using System.Net;

namespace Lifx.Api.Test.Lan;

/// <summary>
/// Stand-in details for a LIFX device the LAN tests construct but never actually contact.
/// </summary>
/// <remarks>
/// The host is loopback rather than a made-up address on someone's home subnet: the tests only
/// need a well-formed host name, and if one ever does open a socket nothing leaves the machine.
/// </remarks>
internal static class LanTestDevice
{
	/// <summary>
	/// Gets the host name used for test devices.
	/// </summary>
	public static string HostName { get; } = IPAddress.Loopback.ToString();

	/// <summary>
	/// Gets a well-formed six byte MAC address for test devices.
	/// </summary>
	public static byte[] MacAddress => [0xD0, 0x73, 0xD5, 0x00, 0x00, 0x01];
}
