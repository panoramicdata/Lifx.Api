namespace Lifx.Api.Models.Lan;

/// <summary>
/// Represents the LightBulb type.
/// </summary>
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
