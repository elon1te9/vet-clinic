using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface IAdminDashboardService
{
    Task<AdminDashboardResponse> GetDashboardAsync();
    Task<AdminDashboardResponse> GetTodayAsync();
}
