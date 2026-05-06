using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class HospitalizationApiService
{
    private readonly ApiRequestService _apiRequestService;

    public HospitalizationApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<List<HospitalizationResponse>> GetAllAsync()
    {
        return await _apiRequestService.GetAsync<List<HospitalizationResponse>>("api/hospitalizations") ?? [];
    }

    public async Task<List<HospitalizationResponse>> GetActiveAsync()
    {
        return await _apiRequestService.GetAsync<List<HospitalizationResponse>>("api/hospitalizations/active") ?? [];
    }

    public async Task<HospitalizationResponse?> GetByIdAsync(int id)
    {
        return await _apiRequestService.GetAsync<HospitalizationResponse>($"api/hospitalizations/{id}");
    }

    public async Task<List<HospitalizationResponse>> GetByPetAsync(int petId)
    {
        return await _apiRequestService.GetAsync<List<HospitalizationResponse>>($"api/hospitalizations/pet/{petId}") ?? [];
    }

    public async Task<HospitalizationResponse?> CreateAsync(CreateHospitalizationRequest request)
    {
        return await _apiRequestService.PostAsync<CreateHospitalizationRequest, HospitalizationResponse>("api/hospitalizations", request);
    }

    public async Task<HospitalizationResponse?> UpdateAsync(int id, CreateHospitalizationRequest request)
    {
        return await _apiRequestService.PutAsync<CreateHospitalizationRequest, HospitalizationResponse>($"api/hospitalizations/{id}", request);
    }

    public async Task<HospitalizationResponse?> CloseAsync(int id)
    {
        return await _apiRequestService.PutAsync<object, HospitalizationResponse>($"api/hospitalizations/{id}/close", new { });
    }

    public async Task<List<CareLogResponse>> GetCareLogsAsync(int hospitalizationId)
    {
        return await _apiRequestService.GetAsync<List<CareLogResponse>>($"api/care-logs/hospitalization/{hospitalizationId}") ?? [];
    }

    public async Task<CareLogResponse?> CreateCareLogAsync(CreateCareLogRequest request)
    {
        return await _apiRequestService.PostAsync<CreateCareLogRequest, CareLogResponse>("api/care-logs", request);
    }
}
