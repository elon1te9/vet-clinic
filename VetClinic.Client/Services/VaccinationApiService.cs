using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class VaccinationApiService
{
    private readonly ApiRequestService _apiRequestService;

    public VaccinationApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<List<VaccinationResponse>> GetAllAsync()
    {
        return await _apiRequestService.GetAsync<List<VaccinationResponse>>("api/vaccinations") ?? [];
    }

    public async Task<List<VaccinationResponse>> GetMyAsync()
    {
        return await _apiRequestService.GetAsync<List<VaccinationResponse>>("api/vaccinations/my") ?? [];
    }

    public async Task<VaccinationResponse?> GetByIdAsync(int id)
    {
        return await _apiRequestService.GetAsync<VaccinationResponse>($"api/vaccinations/{id}");
    }

    public async Task<List<VaccinationResponse>> GetByPetAsync(int petId)
    {
        return await _apiRequestService.GetAsync<List<VaccinationResponse>>($"api/vaccinations/pet/{petId}") ?? [];
    }

    public async Task<List<VaccinationResponse>> GetUpcomingAsync()
    {
        return await _apiRequestService.GetAsync<List<VaccinationResponse>>("api/vaccinations/upcoming") ?? [];
    }

    public async Task<List<VaccinationResponse>> GetOverdueAsync()
    {
        return await _apiRequestService.GetAsync<List<VaccinationResponse>>("api/vaccinations/overdue") ?? [];
    }

    public async Task<VaccinationResponse?> CreateAsync(CreateVaccinationRequest request)
    {
        return await _apiRequestService.PostAsync<CreateVaccinationRequest, VaccinationResponse>("api/vaccinations", request);
    }

    public async Task<VaccinationResponse?> UpdateAsync(int id, CreateVaccinationRequest request)
    {
        return await _apiRequestService.PutAsync<CreateVaccinationRequest, VaccinationResponse>($"api/vaccinations/{id}", request);
    }
}
