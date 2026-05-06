using System.Security.Claims;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface IMedicalRecordService
{
    Task<List<MedicalRecordResponse>> GetAllAsync();
    Task<MedicalRecordResponse?> GetByIdAsync(int id, ClaimsPrincipal user);
    Task<List<MedicalRecordResponse>> GetByPetAsync(int petId, ClaimsPrincipal user);
    Task<MedicalRecordResponse?> CreateAsync(CreateMedicalRecordRequest request, ClaimsPrincipal user);
    Task<MedicalRecordResponse?> UpdateAsync(int id, CreateMedicalRecordRequest request, ClaimsPrincipal user);
}
