using System.Security.Claims;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> RegisterOwnerAsync(RegisterOwnerRequest request);
    Task<AuthResponse?> RegisterStaffAsync(RegisterStaffRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> GetCurrentUserAsync(ClaimsPrincipal user);
}
