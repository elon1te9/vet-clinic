using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class ClinicServiceApiService
{
    private readonly ApiRequestService _apiRequestService;

    public ClinicServiceApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<List<ClinicServiceResponse>> GetAllAsync()
    {
        return await _apiRequestService.GetAsync<List<ClinicServiceResponse>>("api/services") ?? [];
    }

    public async Task<ClinicServiceResponse?> CreateAsync(CreateClinicServiceRequest request)
    {
        return await _apiRequestService.PostAsync<CreateClinicServiceRequest, ClinicServiceResponse>("api/services", request);
    }

    public async Task<ClinicServiceResponse?> UpdateAsync(int id, UpdateClinicServiceRequest request)
    {
        return await _apiRequestService.PutAsync<UpdateClinicServiceRequest, ClinicServiceResponse>($"api/services/{id}", request);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _apiRequestService.DeleteAsync($"api/services/{id}");
    }
}
