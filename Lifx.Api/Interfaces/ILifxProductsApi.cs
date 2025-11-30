using Lifx.Api.Models.Cloud.Responses;
using Refit;

namespace Lifx.Api.Interfaces;

/// <summary>
/// Refit interface for LIFX Products operations
/// Fetches from GitHub products repository
/// </summary>
public interface ILifxProductsApi
{
	/// <summary>
	/// Gets all LIFX products from the official product catalog
	/// </summary>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>List of all LIFX vendors and their products</returns>
	[Get("/LIFX/products/refs/heads/master/products.json")]
	Task<List<Vendor>> GetAllAsync(CancellationToken cancellationToken);
}
