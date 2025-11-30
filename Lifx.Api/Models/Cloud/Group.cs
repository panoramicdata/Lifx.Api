using Lifx.Api.Models.Cloud.Responses;
using static Lifx.Api.Models.Cloud.Selector;

namespace Lifx.Api.Models.Cloud;

public sealed class Group(string id, string label, List<Light> lights) : LightCollection(id, label, lights)
{
	public override Selector ToSelector() => new GroupId(Id);
}
