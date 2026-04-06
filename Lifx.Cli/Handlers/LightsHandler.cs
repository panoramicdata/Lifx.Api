using Lifx.Api;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Cloud.Requests;
using Lifx.Api.Models.Cloud.Responses;

namespace Lifx.Cli.Handlers;

/// <summary>
/// Encapsulates the business logic for light control operations.
/// </summary>
public static class LightsHandler
{
	/// <summary>
	/// Builds a SetStateRequest to turn lights on.
	/// </summary>
	public static SetStateRequest BuildOnRequest(double duration)
		=> new() { Power = PowerState.On, Duration = duration };

	/// <summary>
	/// Builds a SetStateRequest to turn lights off.
	/// </summary>
	public static SetStateRequest BuildOffRequest(double duration)
		=> new() { Power = PowerState.Off, Duration = duration };

	/// <summary>
	/// Builds a TogglePowerRequest.
	/// </summary>
	public static TogglePowerRequest BuildToggleRequest(double duration)
		=> new() { Duration = duration };

	/// <summary>
	/// Builds a SetStateRequest for color change.
	/// </summary>
	public static SetStateRequest BuildColorRequest(string color, double duration)
		=> new() { Color = color, Duration = duration };

	/// <summary>
	/// Validates and builds a SetStateRequest for brightness change.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when brightness is outside 0.0–1.0.</exception>
	public static SetStateRequest BuildBrightnessRequest(double brightness, double duration)
	{
		if (brightness is < 0 or > 1)
		{
			throw new ArgumentOutOfRangeException(nameof(brightness), brightness, "Brightness must be between 0.0 and 1.0.");
		}

		return new SetStateRequest { Brightness = brightness, Duration = duration };
	}

	/// <summary>
	/// Lists lights for the given selector.
	/// </summary>
	public static async Task<List<Light>> ListLightsAsync(ILifxClient client, Selector selector, CancellationToken cancellationToken)
	{
		var lights = await client.Lights.ListLightsAsync(selector.ToString(), cancellationToken);
		return lights.Where(a => a.LastSeen is not null).ToList();
	}
}
