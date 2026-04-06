using Lifx.Api;
using Lifx.Api.Models.Cloud.Requests;
using Lifx.Api.Models.Cloud.Responses;

namespace Lifx.Cli.Handlers;

/// <summary>
/// Encapsulates the business logic for scene operations.
/// </summary>
public static class ScenesHandler
{
	/// <summary>
	/// Finds a scene by name or UUID (case-insensitive).
	/// </summary>
	/// <returns>The matching scene, or null if not found.</returns>
	public static Scene? FindScene(IReadOnlyList<Scene> scenes, string nameOrUuid)
		=> scenes.FirstOrDefault(s =>
			s.Name.Equals(nameOrUuid, StringComparison.OrdinalIgnoreCase) ||
			s.Uuid.Equals(nameOrUuid, StringComparison.OrdinalIgnoreCase));

	/// <summary>
	/// Builds the scene selector string for the LIFX API.
	/// </summary>
	public static string BuildSceneSelector(string uuid)
		=> $"scene_id:{uuid}";

	/// <summary>
	/// Builds an ActivateSceneRequest.
	/// </summary>
	public static ActivateSceneRequest BuildActivateRequest(double duration, bool fast)
		=> new() { Duration = duration, Fast = fast };
}
