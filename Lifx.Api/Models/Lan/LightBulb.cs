namespace Lifx.Api.Models.Lan;

/// <summary>
/// LIFX light bulb
/// </summary>
/// <remarks>
/// Initializes a new instance of a bulb instead of relying on discovery. At least the host name must be provide for the device to be usable.
/// </remarks>
/// <param name="hostname">Required</param>
/// <param name="macAddress"></param>
/// <param name="service"></param>
/// <param name="port"></param>
public sealed class LightBulb(
	string hostname,
	byte[] macAddress,
	byte service = 0,
	uint port = 0)
	: Device(
		hostname,
		macAddress,
		service,
		port)
{
}
