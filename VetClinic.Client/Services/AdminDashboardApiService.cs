using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class AdminDashboardApiService
{
    private readonly ApiRequestService _apiRequestService;

    public AdminDashboardApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<AdminDashboardResponse?> GetDashboardAsync()
    {
        return await _apiRequestService.GetAsync<AdminDashboardResponse>("api/admin/dashboard");
    }

    public async Task<AdminDashboardResponse?> GetTodayAsync()
    {
        return await _apiRequestService.GetAsync<AdminDashboardResponse>("api/admin/dashboard/today");
    }
}
