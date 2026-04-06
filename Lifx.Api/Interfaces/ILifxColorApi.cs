using Lifx.Api.Models.Cloud.Responses;
using Refit;

namespace Lifx.Api.Interfaces;

/// <summary>
/// Defines the ILifxColorApi contract.
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
