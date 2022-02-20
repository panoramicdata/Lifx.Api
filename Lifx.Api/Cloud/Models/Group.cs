using Lifx.Api.Cloud.Clients;
using Lifx.Api.Cloud.Models.Response;
using static Lifx.Api.Cloud.Models.Selector;

namespace Lifx.Api.Cloud.Models
{
    public sealed class Group : LightCollection
    {
        public Group(LifxCloudClient client, string id, string label, List<Light> lights)
            : base(client, id, label, lights) { }

        public override Selector ToSelector()
        {
            return new GroupId(Id);
        }
    }
}
