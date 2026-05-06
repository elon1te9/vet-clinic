using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class NotificationApiService
{
    private readonly ApiRequestService _apiRequestService;

    public NotificationApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<List<NotificationResponse>> GetMyAsync()
    {
        return await _apiRequestService.GetAsync<List<NotificationResponse>>("api/notifications/my") ?? [];
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        return await _apiRequestService.PutAsync($"api/notifications/{id}/read", new { });
    }

    public async Task<bool> MarkAllAsReadAsync()
    {
        return await _apiRequestService.PutAsync("api/notifications/read-all", new { });
    }
}
