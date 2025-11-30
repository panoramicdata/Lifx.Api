using Lifx.Api.Models.Cloud.Requests;
using Lifx.Api.Models.Cloud.Responses;
using Refit;

namespace Lifx.Api.Interfaces;

/// <summary>
/// Refit interface for LIFX Scenes operations
/// </summary>
public interface ILifxScenesApi
{
	/// <summary>
	/// Lists scenes belonging to the authenticated account
	/// </summary>
	[Get("/scenes")]
	Task<List<Scene>> ListScenesAsync(
		CancellationToken cancellationToken);

	/// <summary>
	/// Activates a scene
	/// </summary>
	[Put("/scenes/{sceneSelector}/activate")]
	Task<IApiResponse<SuccessResponse>> ActivateSceneAsync(
		string sceneSelector,
		[Body] ActivateSceneRequest request,
		CancellationToken cancellationToken);
}
