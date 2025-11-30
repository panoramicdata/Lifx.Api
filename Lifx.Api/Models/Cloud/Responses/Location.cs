namespace Lifx.Api.Models.Cloud.Responses;

public sealed class Location(string id, string label, List<Light> lights) : LightCollection(id, label, lights)
{
	public override Selector ToSelector() => new Selector.LocationId(Id);
}
