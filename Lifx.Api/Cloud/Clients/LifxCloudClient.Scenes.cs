using Lifx.Api.Cloud.Models.Request;
using Lifx.Api.Cloud.Models.Response;

namespace Lifx.Api.Cloud.Clients
{
    public partial class LifxCloudClient
    {
        public async Task<List<Scene>> ListScenes()
        {
            var response = await GetResponseData<ApiResponse>($"{SceneEndPoint}");

            if (response.GetType() == typeof(ErrorResponse))
            {
                throw new Exception(((ErrorResponse)response).Error);
            }
            else
            {
                return (List<Scene>)response;
            }
        }

        public async Task<Scene> GetScene(string selector)
        {
            var response = await GetResponseData<ApiResponse>($"{SceneEndPoint}{selector}");

            if (response.GetType() == typeof(ErrorResponse))
            {
                throw new Exception(((ErrorResponse)response).Error);
            }
            else
            {
                return (Scene)response;
            }
        }

        public async Task<ApiResponse> ActivateScene(string scene_uuid, SetStateRequest request)
        {
            return await PutResponseData<ApiResponse>($"{SceneEndPoint}scene_id:{scene_uuid}/activate", request);
        }
    }
}
