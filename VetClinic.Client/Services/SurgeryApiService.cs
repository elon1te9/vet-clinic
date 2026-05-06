using VetClinic.Shared.Enums;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class SurgeryApiService
{
    private readonly ApiRequestService _apiRequestService;

    public SurgeryApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<List<SurgeryResponse>> GetAllAsync()
    {
        return await _apiRequestService.GetAsync<List<SurgeryResponse>>("api/surgeries") ?? [];
    }

    public async Task<SurgeryResponse?> GetByIdAsync(int id)
    {
        return await _apiRequestService.GetAsync<SurgeryResponse>($"api/surgeries/{id}");
    }

    public async Task<List<SurgeryResponse>> GetByPetAsync(int petId)
    {
        return await _apiRequestService.GetAsync<List<SurgeryResponse>>($"api/surgeries/pet/{petId}") ?? [];
    }

    public async Task<SurgeryResponse?> CreateAsync(CreateSurgeryRequest request)
    {
        return await _apiRequestService.PostAsync<CreateSurgeryRequest, SurgeryResponse>("api/surgeries", request);
    }

    public async Task<SurgeryResponse?> UpdateAsync(int id, CreateSurgeryRequest request)
    {
        return await _apiRequestService.PutAsync<CreateSurgeryRequest, SurgeryResponse>($"api/surgeries/{id}", request);
    }

    public async Task<SurgeryResponse?> UpdateStatusAsync(int id, SurgeryStatus status)
    {
        return await _apiRequestService.PutAsync<object, SurgeryResponse>($"api/surgeries/{id}/status?status={status}", new { });
    }
}
