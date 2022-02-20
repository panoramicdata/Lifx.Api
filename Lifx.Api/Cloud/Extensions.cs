using Lifx.Api.Cloud.Clients;
using Lifx.Api.Cloud.Models;
using Lifx.Api.Cloud.Models.Response;

namespace Lifx.Api.Cloud
{
    public enum MatchMode { Any, All }

    public static class Extensions
    {
        public static List<Group> AsGroups(this IEnumerable<Light> lights)
        {
            Dictionary<CollectionSpec, List<Light>> groups =
                new();
            foreach (Light light in lights)
            {
                if (!groups.ContainsKey(light.group))
                {
                    groups[light.group] = new List<Light>();
                }

                groups[light.group].Add(light);
            }
            // Grab client from a light
            LifxCloudClient client = (groups.Count > 0) ? groups.First().Value.First().Client : null;
            return groups.Select(entry => new Group(client, entry.Key.id, entry.Key.name, entry.Value)).ToList();
        }
        public static List<Location> AsLocations(this IEnumerable<Light> lights)
        {
            Dictionary<CollectionSpec, List<Light>> groups =
                new();
            foreach (Light light in lights)
            {
                if (!groups.ContainsKey(light.location))
                {
                    groups[light.location] = new List<Light>();
                }

                groups[light.location].Add(light);
            }
            // Grab client from a light
            LifxCloudClient client = (groups.Count > 0) ? groups.First().Value.First().Client : null;
            return groups.Select(entry => new Location(client, entry.Key.id, entry.Key.name, entry.Value)).ToList();
        }

        public static bool IsSuccessful(SuccessResponse results, MatchMode matchMode = MatchMode.Any)
        {
            return matchMode switch
            {
                MatchMode.All => results.Results.All(a => a.IsSuccessful),
                _ => results.Results.Any(a => a.IsSuccessful),
            };
        }
    }
}
