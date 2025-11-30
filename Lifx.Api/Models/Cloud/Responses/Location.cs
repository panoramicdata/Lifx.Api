namespace Lifx.Api.Models.Cloud.Responses;

public sealed class Location(LifxClient? client, string id, string label, List<Light> lights) : LightCollection(client, id, label, lights)
{
	public override Selector ToSelector() => new Selector.LocationId(Id);
}
