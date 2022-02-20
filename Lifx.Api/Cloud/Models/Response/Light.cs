using Lifx.Api.Cloud.Clients;
using Lifx.Api.Cloud.Models.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static Lifx.Api.Cloud.Models.Selector;

namespace Lifx.Api.Cloud.Models.Response
{
    /// <summary>
    /// Model object for a Light
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Light : ILightTarget<ApiResponse>
    {
        public const string ColorCapability = "has_color";
        public const string ColorTemperatureCapability = "has_variable_color_temp";

        internal LifxCloudClient Client { get; set; }
        /// <summary>
        /// Serial number of the light
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("uuid")]
        public string Uuid { get; private set; }

        [JsonProperty("label")]
        public string Label { get; private set; }

        [JsonProperty("connected")]
        public bool IsConnected { get; private set; }

        public bool IsOn { get { return PowerState == PowerState.On; } }
        [JsonProperty("power")]
        public PowerState PowerState { get; private set; }

        [JsonProperty("color")]
        public Hsbk Color
        {
            get { return color?.WithBrightness(Brightness); }
            set
            {
                color = value;
            }
        }

        [JsonProperty("brightness")]
        public float Brightness { get; private set; }

        [JsonProperty("group")]
        internal CollectionSpec group = new();
        public string GroupId { get { return group.id; } }
        public string GroupName { get { return group.name; } }

        [JsonProperty("location")]
        internal CollectionSpec location = new();
        public string LocationId { get { return location.id; } }
        public string LocationName { get { return location.name; } }

        [JsonProperty("last_seen")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? LastSeen { get; private set; }

        [JsonProperty("seconds_since_seen")]
        public float SecondsSinceSeen { get; private set; }

        [JsonProperty("product_name")]
        public string ProductName { get; private set; }

        [JsonProperty("capabilities")]
        private readonly Dictionary<string, bool> capabilities;

        private Hsbk color;

        public IEnumerable<string> Capabilities
        {
            get
            {
                if (capabilities != null)
                {
                    foreach (var entry in capabilities)
                    {
                        if (entry.Value)
                        {
                            yield return entry.Key;
                        }
                    }
                }
            }
        }

        public bool HasCapability(string capabilitity)
        {
            return capabilities != null && capabilities.ContainsKey(capabilitity) && capabilities[capabilitity];
        }

        public async Task<ApiResponse> TogglePower(TogglePowerRequest request)
        {
            return (await Client.TogglePower(this, request));
        }

        public async Task<ApiResponse> SetState(SetStateRequest request)
        {
            if (Client == null) { return new ApiResponse(); }

            return await Client.SetState(this, request);
        }


        /// <summary>
        /// Re-requests light information
        /// </summary>
        /// <returns>A new instance of this light returned from API</returns>
        public async Task<Light> GetRefreshed()
        {
            return (await Client.ListLights(this)).First();
        }

        /// <summary>
        /// Re-requests light information and updates all properties
        /// </summary>
        public async Task Refresh()
        {
            Light light = await GetRefreshed();
            Id = light.Id;
            Uuid = light.Uuid;
            Label = light.Label;
            IsConnected = light.IsConnected;
            PowerState = light.PowerState;
            Color = light.Color;
            Brightness = light.Brightness;
            group = light.group;
            location = light.location;
            LastSeen = light.LastSeen;
            SecondsSinceSeen = light.SecondsSinceSeen;
            ProductName = light.ProductName;
        }

        public override string ToString()
        {
            return Label;
        }

        public static implicit operator Selector(Light light)
        {
            return new LightId(light.Id);
        }
    }
}
