using System.Security.Claims;
using VetClinic.Shared.Enums;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface ISurgeryService
{
    Task<List<SurgeryResponse>> GetAllAsync(ClaimsPrincipal user);
    Task<SurgeryResponse?> GetByIdAsync(int id, ClaimsPrincipal user);
    Task<List<SurgeryResponse>> GetByPetAsync(int petId, ClaimsPrincipal user);
    Task<SurgeryResponse?> CreateAsync(CreateSurgeryRequest request, ClaimsPrincipal user);
    Task<SurgeryResponse?> UpdateAsync(int id, CreateSurgeryRequest request, ClaimsPrincipal user);
    Task<SurgeryResponse?> UpdateStatusAsync(int id, SurgeryStatus status, ClaimsPrincipal user);
}
