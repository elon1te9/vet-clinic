using System.Security.Claims;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface IHospitalizationService
{
    Task<List<HospitalizationResponse>> GetAllAsync();
    Task<List<HospitalizationResponse>> GetActiveAsync();
    Task<HospitalizationResponse?> GetByIdAsync(int id);
    Task<List<HospitalizationResponse>> GetByPetAsync(int petId);
    Task<HospitalizationResponse?> CreateAsync(CreateHospitalizationRequest request);
    Task<HospitalizationResponse?> UpdateAsync(int id, CreateHospitalizationRequest request);
    Task<HospitalizationResponse?> CloseAsync(int id);
    Task<List<CareLogResponse>> GetCareLogsAsync(int hospitalizationId);
    Task<CareLogResponse?> CreateCareLogAsync(CreateCareLogRequest request, ClaimsPrincipal user);
}
