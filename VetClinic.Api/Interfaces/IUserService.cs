using System.Security.Claims;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetUsersAsync();
    Task<UserResponse?> GetUserByIdAsync(string id);
    Task<bool> BlockUserAsync(string id);
    Task<UserResponse?> UpdateUserRoleAsync(string id, UpdateUserRoleRequest request);
    Task<List<UserResponse>> GetOwnersAsync();
    Task<UserResponse?> GetMyOwnerAsync(ClaimsPrincipal user);
    Task<UserResponse?> GetOwnerByIdAsync(string id);
    Task<List<UserResponse>> GetStaffAsync();
    Task<List<UserResponse>> GetDoctorsAsync();
    Task<List<UserResponse>> GetAssistantsAsync();
    Task<UserResponse?> GetStaffByIdAsync(string id);
}
