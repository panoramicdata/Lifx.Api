using Lifx.Api.Interfaces;
using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Cloud.Responses;

namespace Lifx.Api.Extensions;

/// <summary>
/// Represents the LifxLightsApiExtensions type.
/// </summary>
public static class LifxLightsApiExtensions
{
	/// <summary>
	/// Performs ListAsync operation.
	/// </summary>
	public static async Task<List<Light>> ListAsync(this ILifxLightsApi api, Selector selector, CancellationToken cancellationToken)
	{
		var lights = await api.ListLightsAsync(selector.ToString(), cancellationToken);
		var filteredLights = lights.Where(a => a.LastSeen is not null).ToList();
		return filteredLights;
	}

	/// <summary>
	/// Performs ListGroupsAsync operation.
	/// </summary>
	public static Task<List<Group>> ListGroupsAsync(this ILifxLightsApi api, Selector selector, CancellationToken cancellationToken)
		=> GroupLightsAsync(
			api,
			selector,
			light => light.Group,
			(id, name, lights) => new Group(id, name, lights),
			cancellationToken);

	/// <summary>
	/// Performs ListLocationsAsync operation.
	/// </summary>
	public static Task<List<Location>> ListLocationsAsync(this ILifxLightsApi api, Selector selector, CancellationToken cancellationToken)
		=> GroupLightsAsync(
			api,
			selector,
			light => light.Location,
			(id, name, lights) => new Location(id, name, lights),
			cancellationToken);

	/// <summary>
	/// Buckets the selected lights by one of their collection properties.
	/// </summary>
	/// <remarks>
	/// Groups and locations are the same operation over a different property of the light and a
	/// different result type, so both go through here rather than repeating the bucketing.
	/// </remarks>
	private static async Task<List<TCollection>> GroupLightsAsync<TCollection>(
		ILifxLightsApi api,
		Selector selector,
		Func<Light, CollectionSpec> keySelector,
		Func<string, string, List<Light>, TCollection> create,
		CancellationToken cancellationToken)
	{
		var lights = await api.ListAsync(selector, cancellationToken);

		Dictionary<CollectionSpec, List<Light>> collections = [];
		foreach (var light in lights)
		{
			var key = keySelector(light);
			if (!collections.TryGetValue(key, out List<Light>? value))
			{
				value = [];
				collections[key] = value;
			}

			value.Add(light);
		}

		return [.. collections.Select(entry => create(entry.Key.Id, entry.Key.Name, entry.Value))];
	}
}
