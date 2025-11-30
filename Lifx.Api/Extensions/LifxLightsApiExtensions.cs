using Lifx.Api.Interfaces;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Cloud.Requests;
using Lifx.Api.Models.Cloud.Responses;

namespace Lifx.Api.Extensions;

/// <summary>
/// Extension methods for ILifxLightsApi
/// </summary>
public static class LifxLightsApiExtensions
{
	private static LifxClient? _client;

	/// <summary>
	/// Sets the client for attaching to lights
	/// </summary>
	internal static void SetClient(LifxClient client) => _client = client;

	/// <summary>
	/// Lists lights belonging to the authenticated account
	/// </summary>
	public static async Task<List<Light>> ListAsync(this ILifxLightsApi api, Selector selector, CancellationToken cancellationToken)
	{
		var lights = await api.ListLightsAsync(selector.ToString(), cancellationToken);
		var filteredLights = lights.Where(a => a.LastSeen is not null).ToList();

		// Attach client to lights
		foreach (var light in filteredLights)
		{
			light.Client = _client;
		}

		return filteredLights;
	}

	/// <summary>
	/// Gets light groups belonging to the authenticated account
	/// </summary>
	public static async Task<List<Group>> ListGroupsAsync(this ILifxLightsApi api, Selector selector, CancellationToken cancellationToken)
		=> (await api.ListAsync(selector, cancellationToken)).AsGroups();

	/// <summary>
	/// Gets locations belonging to the authenticated account
	/// </summary>
	public static async Task<List<Location>> ListLocationsAsync(this ILifxLightsApi api, Selector selector, CancellationToken cancellationToken)
		=> (await api.ListAsync(selector, cancellationToken)).AsLocations();

	/// <summary>
	/// Sets state for lights matching the selector
	/// </summary>
	public static Task<ApiResponse> SetStateAsync(this ILifxLightsApi api, Selector selector, SetStateRequest request, CancellationToken cancellationToken)
		=> api.SetStateAsync(selector.ToString(), request, cancellationToken);

	/// <summary>
	/// Adjusts the state delta
	/// </summary>
	public static Task<ApiResponse> StateDeltaAsync(this ILifxLightsApi api, Selector selector, StateDeltaRequest request, CancellationToken cancellationToken)
		=> api.StateDeltaAsync(selector.ToString(), request, cancellationToken);

	/// <summary>
	/// Toggles power for lights matching the selector
	/// </summary>
	public static Task<ApiResponse> TogglePowerAsync(this ILifxLightsApi api, Selector selector, TogglePowerRequest request, CancellationToken cancellationToken)
		=> api.TogglePowerAsync(selector.ToString(), request, cancellationToken);

	/// <summary>
	/// Cycles through states
	/// </summary>
	public static Task<ApiResponse> CycleAsync(this ILifxLightsApi api, Selector selector, CycleRequest request, CancellationToken cancellationToken)
		=> api.CycleAsync(selector.ToString(), request, cancellationToken);

	/// <summary>
	/// Runs the HEV clean cycle
	/// </summary>
	public static Task<ApiResponse> CleanAsync(this ILifxLightsApi api, Selector selector, CleanRequest request, CancellationToken cancellationToken)
		=> api.CleanAsync(selector.ToString(), request, cancellationToken);
}
