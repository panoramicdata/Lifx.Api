using Lifx.Api.Cloud.Models;
using Lifx.Api.Cloud.Models.Request;
using Lifx.Api.Cloud.Models.Response;

namespace Lifx.Api.Cloud.Clients
{
    public partial class LifxCloudClient
    {
        public async Task<List<Light>> ListLights(Selector? selector = null)
        {
            if (selector == null) { selector = Selector.All; }

            var response = await GetResponseData<List<Light>>($"{LightEndPoint}{selector}");

            if (response.GetType() == typeof(ErrorResponse))
            {
                throw new Exception(((ErrorResponse)response).Error);
            }
            else
            {
                var lights = (List<Light>)response;
                lights = lights.Where(a => a.LastSeen != null).ToList();
                foreach (var light in lights)
                {
                    light.Client = this;
                }

                return lights;
            }
        }

        /// <summary>
        /// Gets light groups belonging to the authenticated account
        /// </summary>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <returns>All groups containing matching lights</returns>
        public async Task<List<Group>> ListGroups(Selector? selector = null)
        {
            return (await ListLights(selector)).AsGroups();
        }
        /// <summary>
        /// Gets locations belonging to the authenticated account
        /// </summary>
        /// <param name="selector">Filter for which lights are targetted</param>
        /// <returns>All locations containing matching lights</returns>
        public async Task<List<Location>> ListLocations(Selector? selector = null)
        {
            return (await ListLights(selector)).AsLocations();
        }

        public async Task<ApiResponse> SetState(Selector selector, SetStateRequest request)
        {
            return await PutResponseData<ApiResponse>($"{LightEndPoint}{selector}/state", request);
        }

        public async Task<ApiResponse> StateDelta(Selector selector, StateDeltaRequest request)
        {
            return await PostResponseData<ApiResponse>($"{LightEndPoint}{selector}/state/delta", request);
        }

        public async Task<ApiResponse> TogglePower(Selector selector, TogglePowerRequest request)
        {
            return await PostResponseData<ApiResponse>($"{LightEndPoint}{selector}/toggle", request);
        }

        public async Task<ApiResponse> BreathEffect(Selector selector, BreatheEffectRequest request)
        {
            return await PostResponseData<ApiResponse>($"{LightEndPoint}{selector}/effects/breathe", request);
        }

        public async Task<ApiResponse> MoveEffect(Selector selector, MoveEffectRequest request)
        {
            return await PostResponseData<ApiResponse>($"{LightEndPoint}{selector}/effects/move", request);
        }

        public async Task<ApiResponse> MorphEffect(Selector selector, MorphEffectRequest request)
        {
            return await PostResponseData<ApiResponse>($"{LightEndPoint}{selector}/effects/morph", request);
        }

        public async Task<ApiResponse> FlameEffect(Selector selector, FlameEffectRequest request)
        {
            return await PostResponseData<ApiResponse>($"{LightEndPoint}{selector}/effects/flame", request);
        }

        public async Task<ApiResponse> PulseEffect(Selector selector, PulseEffectRequest request)
        {
            return await PostResponseData<ApiResponse>($"{LightEndPoint}{selector}/effects/pulse", request);
        }

        public async Task<ApiResponse> EffectsOff(Selector selector, EffectsOffRequest request)
        {
            return await PostResponseData<ApiResponse>($"{LightEndPoint}{selector}/effects/off", request);
        }

        public async Task<ApiResponse> Cycle(Selector selector, CycleRequest request)
        {
            return await PostResponseData<ApiResponse>($"{LightEndPoint}{selector}/cycle", request);
        }
    }
}
