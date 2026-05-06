using Microsoft.AspNetCore.SignalR.Client;
using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class SignalRNotificationService : IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private readonly LocalStorageService _localStorageService;
    private HubConnection? _connection;

    public event Action<NotificationResponse>? NotificationReceived;

    public SignalRNotificationService(HttpClient httpClient, LocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
    }

    public async Task StartAsync()
    {
        if (_connection is not null)
        {
            return;
        }

        var hubUrl = new Uri(_httpClient.BaseAddress!, "/hubs/notifications").ToString();

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = async () => await _localStorageService.GetItemAsync("token");
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.On<NotificationResponse>("ReceiveNotification", notification =>
        {
            NotificationReceived?.Invoke(notification);
        });

        try
        {
            await _connection.StartAsync();
        }
        catch
        {
            _connection = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }
}
