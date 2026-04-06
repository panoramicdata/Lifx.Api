namespace Lifx.Api.Models.Cloud.Responses;

/// <summary>
/// Represents the Location type.
/// </summary>
public sealed class Location(string id, string label, List<Light> lights) : LightCollection(id, label, lights)
{
	/// <summary>
	/// Performs ToSelector operation.
	/// </summary>
	public override Selector ToSelector() => new Selector.LocationId(Id);
}
