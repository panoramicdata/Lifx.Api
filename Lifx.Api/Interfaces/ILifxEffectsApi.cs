using Lifx.Api.Models.Cloud.Requests;
using Lifx.Api.Models.Cloud.Responses;
using Refit;

namespace Lifx.Api.Interfaces;

/// <summary>
/// Refit interface for LIFX Effects operations
/// </summary>
public interface ILifxEffectsApi
{
	/// <summary>
	/// Performs a breathe effect
	/// </summary>
	[Post("/lights/{selector}/effects/breathe")]
	Task<ApiResponse> BreatheAsync(
		string selector,
		[Body] BreatheEffectRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Performs a move effect
	/// </summary>
	[Post("/lights/{selector}/effects/move")]
	Task<ApiResponse> MoveAsync(
		string selector,
		[Body] MoveEffectRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Performs a morph effect
	/// </summary>
	[Post("/lights/{selector}/effects/morph")]
	Task<ApiResponse> MorphAsync(
		string selector,
		[Body] MorphEffectRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Performs a flame effect
	/// </summary>
	[Post("/lights/{selector}/effects/flame")]
	Task<ApiResponse> FlameAsync(
		string selector,
		[Body] FlameEffectRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Performs a pulse effect
	/// </summary>
	[Post("/lights/{selector}/effects/pulse")]
	Task<ApiResponse> PulseAsync(
		string selector,
		[Body] PulseEffectRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Performs a clouds effect
	/// </summary>
	[Post("/lights/{selector}/effects/clouds")]
	Task<ApiResponse> CloudsAsync(
		string selector,
		[Body] CloudsEffectRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Performs a sunrise effect
	/// </summary>
	[Post("/lights/{selector}/effects/sunrise")]
	Task<ApiResponse> SunriseAsync(
		string selector,
		[Body] SunriseEffectRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Performs a sunset effect
	/// </summary>
	[Post("/lights/{selector}/effects/sunset")]
	Task<ApiResponse> SunsetAsync(
		string selector,
		[Body] SunsetEffectRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Turns off effects
	/// </summary>
	[Post("/lights/{selector}/effects/off")]
	Task<ApiResponse> OffAsync(
		string selector,
		[Body] EffectsOffRequest request,
		CancellationToken cancellationToken);
}
