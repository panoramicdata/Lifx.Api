using Lifx.Api.Models.Cloud.Requests;
using Lifx.Api.Models.Cloud.Responses;

namespace Lifx.Api.Interfaces;

public interface ILightTarget<ResponseType>
{
	string Id { get; }

	string Label { get; }

	bool IsOn { get; }

	Task<ApiResponse> TogglePower(TogglePowerRequest request);

	Task<ApiResponse> SetState(SetStateRequest request);
}
