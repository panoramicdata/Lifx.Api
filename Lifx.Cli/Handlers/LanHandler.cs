namespace Lifx.Cli.Handlers;

/// <summary>
/// Encapsulates the business logic for LAN operations.
/// </summary>
public static class LanHandler
{
	/// <summary>
	/// Normalizes a MAC address to uppercase colon-separated format.
	/// </summary>
	public static string NormalizeMacAddress(string macAddress)
		=> macAddress.ToUpperInvariant().Replace("-", ":").Replace(".", ":");

	/// <summary>
	/// Validates a Kelvin temperature for LIFX lights.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when kelvin is outside 2500–9000.</exception>
	public static void ValidateKelvin(int kelvin)
	{
		if (kelvin is < 2500 or > 9000)
		{
			throw new ArgumentOutOfRangeException(nameof(kelvin), kelvin, "Kelvin must be between 2500 and 9000.");
		}
	}

	/// <summary>
	/// Validates a light name for the rename operation.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown when name is empty or exceeds 32 characters.</exception>
	public static void ValidateLightName(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Name cannot be empty.", nameof(name));
		}

		if (name.Length > 32)
		{
			throw new ArgumentException("Name must be 32 characters or less.", nameof(name));
		}
	}
}
