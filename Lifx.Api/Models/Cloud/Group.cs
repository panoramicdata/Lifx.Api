using Lifx.Api.Models.Cloud.Responses;
using static Lifx.Api.Models.Cloud.Selector;

namespace Lifx.Api.Models.Cloud;

/// <summary>
/// Represents the Group type.
/// </summary>
public sealed class Group(string id, string label, List<Light> lights) : LightCollection(id, label, lights)
{
	/// <summary>
	/// Performs ToSelector operation.
	/// </summary>
	public override Selector ToSelector() => new GroupId(Id);
}
