using System.Security.Claims;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface IPetService
{
    Task<List<PetResponse>> GetAllAsync();
    Task<List<PetResponse>> GetMyAsync(ClaimsPrincipal user);
    Task<PetResponse?> GetByIdAsync(int id, ClaimsPrincipal user);
    Task<PetResponse?> CreateAsync(CreatePetRequest request, ClaimsPrincipal user);
    Task<PetResponse?> UpdateAsync(int id, UpdatePetRequest request, ClaimsPrincipal user);
    Task<bool> DeleteAsync(int id, ClaimsPrincipal user);
}
