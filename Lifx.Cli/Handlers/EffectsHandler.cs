using Lifx.Api.Models.Cloud.Requests;

namespace Lifx.Cli.Handlers;

/// <summary>
/// Encapsulates the business logic for building effect requests.
/// </summary>
public static class EffectsHandler
{
	/// <summary>
	/// Builds a BreatheEffectRequest.
	/// </summary>
	public static BreatheEffectRequest BuildBreatheRequest(string color, double period, double cycles)
		=> new() { Color = color, Period = period, Cycles = cycles, PowerOn = true };

	/// <summary>
	/// Builds a PulseEffectRequest.
	/// </summary>
	public static PulseEffectRequest BuildPulseRequest(string color, double period, double cycles)
		=> new() { Color = color, Period = period, Cycles = cycles, PowerOn = true };

	/// <summary>
	/// Builds a MorphEffectRequest.
	/// </summary>
	public static MorphEffectRequest BuildMorphRequest(double period, double duration)
		=> new() { Period = period, Duration = duration, PowerOn = true };

	/// <summary>
	/// Builds a FlameEffectRequest.
	/// </summary>
	public static FlameEffectRequest BuildFlameRequest(double period, double duration)
		=> new() { Period = period, Duration = duration, PowerOn = true };

	/// <summary>
	/// Builds a MoveEffectRequest.
	/// </summary>
	public static MoveEffectRequest BuildMoveRequest(string direction, double period)
		=> new() { Direction = direction, Period = period, PowerOn = true };

	/// <summary>
	/// Builds a CloudsEffectRequest.
	/// </summary>
	public static CloudsEffectRequest BuildCloudsRequest(double duration)
		=> new() { Duration = duration, PowerOn = true };

	/// <summary>
	/// Builds a SunriseEffectRequest.
	/// </summary>
	public static SunriseEffectRequest BuildSunriseRequest(double duration)
		=> new() { Duration = duration };

	/// <summary>
	/// Builds a SunsetEffectRequest.
	/// </summary>
	public static SunsetEffectRequest BuildSunsetRequest(double duration)
		=> new() { Duration = duration };

	/// <summary>
	/// Builds an EffectsOffRequest.
	/// </summary>
	public static EffectsOffRequest BuildOffRequest(bool powerOff)
		=> new() { PowerOff = powerOff };
}
