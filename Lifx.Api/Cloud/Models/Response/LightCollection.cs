using System.Collections;
using Lifx.Api.Cloud.Clients;
using Lifx.Api.Cloud.Models.Request;

namespace Lifx.Api.Cloud.Models.Response
{
    public abstract class LightCollection : IEnumerable<Light>, ILightTarget<List<ApiResponse>>
    {
        public string Id { get; private set; }
        public string Label { get; private set; }

        public bool IsOn { get { return lights.Any(l => l.IsOn); } }

        private List<Light> lights;
        private readonly LifxCloudClient client;

        internal LightCollection(LifxCloudClient client, string id, string label, List<Light> lights)
        {
            this.client = client;
            Id = id;
            Label = label;
            this.lights = lights;
        }

        public IEnumerator<Light> GetEnumerator()
        {
            return lights.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return lights.GetEnumerator();
        }

        public async Task Refresh()
        {
            if (client != null)
            {
                lights = await client.ListLights(this);
            }
        }

        public abstract Selector ToSelector();

        public override string ToString()
        {
            return Label;
        }

        public async Task<ApiResponse> TogglePower(TogglePowerRequest request)
        {
            if (client == null) { return new ApiResponse(); }

            return await client.TogglePower(this, request);
        }

        public async Task<ApiResponse> SetState(SetStateRequest request)
        {
            if (client == null) { return new ApiResponse(); }

            return await client.SetState(this,request);
        }

        public static implicit operator Selector(LightCollection lightCollection)
        {
            return lightCollection.ToSelector();
        }
    }
}
