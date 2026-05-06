using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class MedicalRecordApiService
{
    private readonly ApiRequestService _apiRequestService;

    public MedicalRecordApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<List<MedicalRecordResponse>> GetAllAsync()
    {
        return await _apiRequestService.GetAsync<List<MedicalRecordResponse>>("api/medical-records") ?? [];
    }

    public async Task<MedicalRecordResponse?> GetByIdAsync(int id)
    {
        return await _apiRequestService.GetAsync<MedicalRecordResponse>($"api/medical-records/{id}");
    }

    public async Task<List<MedicalRecordResponse>> GetByPetAsync(int petId)
    {
        return await _apiRequestService.GetAsync<List<MedicalRecordResponse>>($"api/medical-records/pet/{petId}") ?? [];
    }

    public async Task<MedicalRecordResponse?> CreateAsync(CreateMedicalRecordRequest request)
    {
        return await _apiRequestService.PostAsync<CreateMedicalRecordRequest, MedicalRecordResponse>("api/medical-records", request);
    }

    public async Task<MedicalRecordResponse?> UpdateAsync(int id, CreateMedicalRecordRequest request)
    {
        return await _apiRequestService.PutAsync<CreateMedicalRecordRequest, MedicalRecordResponse>($"api/medical-records/{id}", request);
    }
}
