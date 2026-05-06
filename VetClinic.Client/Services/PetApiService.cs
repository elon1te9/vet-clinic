using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class PetApiService
{
    private readonly ApiRequestService _apiRequestService;

    public PetApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<List<PetResponse>> GetAllAsync()
    {
        return await _apiRequestService.GetAsync<List<PetResponse>>("api/pets") ?? [];
    }

    public async Task<List<PetResponse>> GetMyAsync()
    {
        return await _apiRequestService.GetAsync<List<PetResponse>>("api/pets/my") ?? [];
    }

    public async Task<PetResponse?> GetByIdAsync(int id)
    {
        return await _apiRequestService.GetAsync<PetResponse>($"api/pets/{id}");
    }

    public async Task<PetResponse?> CreateAsync(CreatePetRequest request)
    {
        return await _apiRequestService.PostAsync<CreatePetRequest, PetResponse>("api/pets", request);
    }

    public async Task<PetResponse?> UpdateAsync(int id, UpdatePetRequest request)
    {
        return await _apiRequestService.PutAsync<UpdatePetRequest, PetResponse>($"api/pets/{id}", request);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _apiRequestService.DeleteAsync($"api/pets/{id}");
    }
}
