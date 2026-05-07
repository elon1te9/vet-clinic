using System.Security.Claims;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface IVaccinationService
{
    Task<List<VaccinationResponse>> GetAllAsync();
    Task<List<VaccinationResponse>> GetMyAsync(ClaimsPrincipal user);
    Task<VaccinationResponse?> GetByIdAsync(int id, ClaimsPrincipal user);
    Task<List<VaccinationResponse>> GetByPetAsync(int petId, ClaimsPrincipal user);
    Task<List<VaccinationResponse>> GetUpcomingAsync(ClaimsPrincipal user);
    Task<List<VaccinationResponse>> GetOverdueAsync(ClaimsPrincipal user);
    Task<VaccinationResponse?> CreateAsync(CreateVaccinationRequest request, ClaimsPrincipal user);
    Task<VaccinationResponse?> UpdateAsync(int id, CreateVaccinationRequest request, ClaimsPrincipal user);
}
