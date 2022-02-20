using Newtonsoft.Json;

namespace Lifx.Api.Cloud.Models.Response
{
    [JsonObject(MemberSerialization.Fields)]
    internal class CollectionSpec
    {
        public string id;
        public string name;
        public override bool Equals(object obj)
        {
            return obj is CollectionSpec spec && spec.id == id && spec.name == name;
        }

        public override int GetHashCode()
        {
            return (id.GetHashCode() * 77) + name.GetHashCode();
        }
    }
}
