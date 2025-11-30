using Lifx.Api.Interfaces;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Cloud.Requests;
using Lifx.Api.Models.Cloud.Responses;

namespace Lifx.Api.Extensions;

/// <summary>
/// Extension methods for ILifxEffectsApi
/// </summary>
public static class LifxEffectsApiExtensions
{
	/// <summary>
	/// Performs a breathe effect
	/// </summary>
	public static Task<ApiResponse> BreatheAsync(this ILifxEffectsApi api, Selector selector, BreatheEffectRequest request, CancellationToken cancellationToken) =>
		api.BreatheAsync(selector.ToString(), request, cancellationToken);

	/// <summary>
	/// Performs a move effect
	/// </summary>
	public static Task<ApiResponse> MoveAsync(this ILifxEffectsApi api, Selector selector, MoveEffectRequest request, CancellationToken cancellationToken) =>
		api.MoveAsync(selector.ToString(), request, cancellationToken);

	/// <summary>
	/// Performs a morph effect
	/// </summary>
	public static Task<ApiResponse> MorphAsync(this ILifxEffectsApi api, Selector selector, MorphEffectRequest request, CancellationToken cancellationToken) =>
		api.MorphAsync(selector.ToString(), request, cancellationToken);

	/// <summary>
	/// Performs a flame effect
	/// </summary>
	public static Task<ApiResponse> FlameAsync(this ILifxEffectsApi api, Selector selector, FlameEffectRequest request, CancellationToken cancellationToken) =>
		api.FlameAsync(selector.ToString(), request, cancellationToken);

	/// <summary>
	/// Performs a pulse effect
	/// </summary>
	public static Task<ApiResponse> PulseAsync(this ILifxEffectsApi api, Selector selector, PulseEffectRequest request, CancellationToken cancellationToken) =>
		api.PulseAsync(selector.ToString(), request, cancellationToken);

	/// <summary>
	/// Performs a clouds effect
	/// </summary>
	public static Task<ApiResponse> CloudsAsync(this ILifxEffectsApi api, Selector selector, CloudsEffectRequest request, CancellationToken cancellationToken) =>
		api.CloudsAsync(selector.ToString(), request, cancellationToken);

	/// <summary>
	/// Performs a sunrise effect
	/// </summary>
	public static Task<ApiResponse> SunriseAsync(this ILifxEffectsApi api, Selector selector, SunriseEffectRequest request, CancellationToken cancellationToken) =>
		api.SunriseAsync(selector.ToString(), request, cancellationToken);

	/// <summary>
	/// Performs a sunset effect
	/// </summary>
	public static Task<ApiResponse> SunsetAsync(this ILifxEffectsApi api, Selector selector, SunsetEffectRequest request, CancellationToken cancellationToken) =>
		api.SunsetAsync(selector.ToString(), request, cancellationToken);

	/// <summary>
	/// Turns off effects
	/// </summary>
	public static Task<ApiResponse> OffAsync(this ILifxEffectsApi api, Selector selector, EffectsOffRequest request, CancellationToken cancellationToken) =>
		api.OffAsync(selector.ToString(), request, cancellationToken);
}
