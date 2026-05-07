using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class NotificationApiService
{
    private readonly ApiRequestService _apiRequestService;

    public event Action? NotificationsChanged;

    public NotificationApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<List<NotificationResponse>> GetMyAsync()
    {
        return await _apiRequestService.GetAsync<List<NotificationResponse>>("api/notifications/my") ?? [];
    }

    public void NotifyNotificationsChanged()
    {
        NotificationsChanged?.Invoke();
    }

    public async Task<bool> MarkAsReadAsync(int id)
    {
        var result = await _apiRequestService.PutAsync($"api/notifications/{id}/read", new { });
        if (result)
        {
            NotifyNotificationsChanged();
        }

        return result;
    }

    public async Task<bool> MarkAllAsReadAsync()
    {
        var result = await _apiRequestService.PutAsync("api/notifications/read-all", new { });
        if (result)
        {
            NotifyNotificationsChanged();
        }

        return result;
    }
}
