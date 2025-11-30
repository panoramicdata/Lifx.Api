using Lifx.Api.Models.Cloud.Requests;
using Lifx.Api.Models.Cloud.Responses;
using Refit;

namespace Lifx.Api.Interfaces;

/// <summary>
/// Refit interface for LIFX Lights operations
/// </summary>
public interface ILifxLightsApi
{
	/// <summary>
	/// Lists lights belonging to the authenticated account
	/// </summary>
	[Get("/lights/{selector}")]
	Task<List<Light>> ListLightsAsync(
		string selector,
		CancellationToken cancellationToken);

	/// <summary>
	/// Sets state for lights matching the selector
	/// </summary>
	[Put("/lights/{selector}/state")]
	Task<ApiResponse> SetStateAsync(
		string selector,
		[Body] SetStateRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Sets states for multiple lights
	/// </summary>
	[Put("/lights/states")]
	Task<ApiResponse> SetStatesAsync(
		[Body] SetStatesRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Adjusts the state delta
	/// </summary>
	[Post("/lights/{selector}/state/delta")]
	Task<ApiResponse> StateDeltaAsync(
		string selector,
		[Body] StateDeltaRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Toggles power for lights matching the selector
	/// </summary>
	[Post("/lights/{selector}/toggle")]
	Task<ApiResponse> TogglePowerAsync(
		string selector,
		[Body] TogglePowerRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Cycles through states
	/// </summary>
	[Post("/lights/{selector}/cycle")]
	Task<ApiResponse> CycleAsync(
		string selector,
		[Body] CycleRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Runs the HEV clean cycle
	/// </summary>
	[Post("/lights/{selector}/clean")]
	Task<ApiResponse> CleanAsync(
		string selector,
		[Body] CleanRequest request,
		CancellationToken cancellationToken);
}
