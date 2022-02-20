using Newtonsoft.Json;

namespace Lifx.Api.Cloud.Models.Response
{
    public class Scene
    {
        [JsonProperty("uuid")]
        public string UUID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("account")]
        public Account Account { get; set; }
        [JsonProperty("sccount")]
        public List<State> States { get; set; }
        [JsonProperty("created_at")]
        public int CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public int UpdatedAt { get; set; }
    }

    public class Account
    {
        [JsonProperty("uuid")]
        public string UUID { get; set; }
    }

    public class State
    {
        [JsonProperty("brightness")]
        public float Brightness { get; set; }
        [JsonProperty("selector")]
        public string Selector { get; set; }
        [JsonProperty("color")]
        public LifxColor Color { get; set; }
    }
}
