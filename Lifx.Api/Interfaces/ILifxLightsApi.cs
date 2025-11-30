using Lifx.Api.Models.Cloud;
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
	Task<SuccessResponse> SetStateAsync(
		Selector selector,
		[Body] SetStateRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Sets states for multiple lights
	/// </summary>
	[Put("/lights/states")]
	Task<SuccessResponse> SetStatesAsync(
		[Body] SetStatesRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Adjusts the state delta
	/// </summary>
	[Post("/lights/{selector}/state/delta")]
	Task<SuccessResponse> StateDeltaAsync(
		Selector selector,
		[Body] StateDeltaRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Toggles power for lights matching the selector
	/// </summary>
	[Post("/lights/{selector}/toggle")]
	Task<SuccessResponse> TogglePowerAsync(
		Selector selector,
		[Body] TogglePowerRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Cycles through states
	/// </summary>
	[Post("/lights/{selector}/cycle")]
	Task<SuccessResponse> CycleAsync(
		Selector selector,
		[Body] CycleRequest request,
		CancellationToken cancellationToken);

	/// <summary>
	/// Runs the HEV clean cycle
	/// </summary>
	[Post("/lights/{selector}/clean")]
	Task<SuccessResponse> CleanAsync(
		Selector selector,
		[Body] CleanRequest request,
		CancellationToken cancellationToken);
}
