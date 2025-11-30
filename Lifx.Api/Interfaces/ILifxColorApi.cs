using Lifx.Api.Models.Cloud.Responses;
using Refit;

namespace Lifx.Api.Interfaces;

/// <summary>
/// Refit interface for LIFX Color operations
/// </summary>
public interface ILifxColorApi
{
	/// <summary>
	/// Validates a color string
	/// </summary>
	[Get("/color")]
	Task<ColorResult> ValidateColorAsync(
		[AliasAs("string")] string color,
		CancellationToken cancellationToken);
}
