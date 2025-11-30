using Lifx.Api.Models.Cloud;
using Lifx.Api.Models.Cloud.Responses;

namespace Lifx.Api.Extensions;

public enum MatchMode { Any, All }

public static class GroupExtensions
{
	public static List<Group> AsGroups(this IEnumerable<Light> lights)
	{
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
		// Grab client from a light
		LifxClient? client = (groups.Count > 0) ? groups.First().Value.First().Client : null;
		return [.. groups.Select(entry => new Group(client, entry.Key.id, entry.Key.name, entry.Value))];
	}

	public static List<Location> AsLocations(this IEnumerable<Light> lights)
	{
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
		// Grab client from a light
		LifxClient? client = (groups.Count > 0) ? groups.First().Value.First().Client : null;
		return [.. groups.Select(entry => new Location(client, entry.Key.id, entry.Key.name, entry.Value))];
	}

	public static bool IsSuccessful(SuccessResponse results, MatchMode matchMode = MatchMode.Any) => matchMode switch
	{
		MatchMode.All => results.Results.All(a => a.IsSuccessful),
		_ => results.Results.Any(a => a.IsSuccessful),
	};
}
