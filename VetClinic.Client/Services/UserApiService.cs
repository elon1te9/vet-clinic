using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class UserApiService
{
    private readonly ApiRequestService _apiRequestService;

    public UserApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<List<UserResponse>> GetUsersAsync()
    {
        return await _apiRequestService.GetAsync<List<UserResponse>>("api/users") ?? [];
    }

    public async Task<UserResponse?> GetUserByIdAsync(string id)
    {
        return await _apiRequestService.GetAsync<UserResponse>($"api/users/{id}");
    }

    public async Task<bool> BlockUserAsync(string id)
    {
        return await _apiRequestService.PutAsync($"api/users/{id}/block", new { });
    }

    public async Task<UserResponse?> UpdateRoleAsync(string id, UpdateUserRoleRequest request)
    {
        return await _apiRequestService.PutAsync<UpdateUserRoleRequest, UserResponse>($"api/users/{id}/role", request);
    }

    public async Task<List<UserResponse>> GetOwnersAsync()
    {
        return await _apiRequestService.GetAsync<List<UserResponse>>("api/owners") ?? [];
    }

    public async Task<UserResponse?> GetMyOwnerAsync()
    {
        return await _apiRequestService.GetAsync<UserResponse>("api/owners/my");
    }

    public async Task<List<UserResponse>> GetDoctorsAsync()
    {
        return await _apiRequestService.GetAsync<List<UserResponse>>("api/staff/doctors") ?? [];
    }
}
