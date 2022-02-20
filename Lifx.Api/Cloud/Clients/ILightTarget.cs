using Lifx.Api.Cloud.Models.Request;
using Lifx.Api.Cloud.Models.Response;

namespace Lifx.Api.Cloud.Clients
{
    public interface ILightTarget<ResponseType>
    {
        string Id { get; }
        string Label { get; }
        bool IsOn { get; }

        public Task<ApiResponse> TogglePower(TogglePowerRequest request);

        public Task<ApiResponse> SetState(SetStateRequest request);
    }
}
