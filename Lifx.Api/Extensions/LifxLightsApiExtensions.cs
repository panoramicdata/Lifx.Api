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
	public static async Task<List<Group>> ListGroupsAsync(this ILifxLightsApi api, Selector selector, CancellationToken cancellationToken)
	{
		var lights = await api.ListAsync(selector, cancellationToken);

		// Group lights by their Group property
		Dictionary<CollectionSpec, List<Light>> groups = [];
		foreach (Light light in lights)
		{
			if (!groups.TryGetValue(light.Group, out List<Light>? value))
			{
				value = [];
				groups[light.Group] = value;
			}

			value.Add(light);
		}

		return [.. groups.Select(entry => new Group(entry.Key.Id, entry.Key.Name, entry.Value))];
	}

	/// <summary>
	/// Performs ListLocationsAsync operation.
	/// </summary>
	public static async Task<List<Location>> ListLocationsAsync(this ILifxLightsApi api, Selector selector, CancellationToken cancellationToken)
	{
		var lights = await api.ListAsync(selector, cancellationToken);

		// Group lights by their Location property
		Dictionary<CollectionSpec, List<Light>> groups = [];
		foreach (Light light in lights)
		{
			if (!groups.TryGetValue(light.Location, out List<Light>? value))
			{
				value = [];
				groups[light.Location] = value;
			}

			value.Add(light);
		}

		return [.. groups.Select(entry => new Location(entry.Key.Id, entry.Key.Name, entry.Value))];
	}
}
