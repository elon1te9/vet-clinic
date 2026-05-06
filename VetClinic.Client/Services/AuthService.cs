using System.Net.Http.Headers;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class AuthService
{
    private readonly ApiRequestService _apiRequestService;
    private readonly LocalStorageService _localStorageService;
    private readonly HttpClient _httpClient;

    public AuthService(
        ApiRequestService apiRequestService,
        LocalStorageService localStorageService,
        HttpClient httpClient)
    {
        _apiRequestService = apiRequestService;
        _localStorageService = localStorageService;
        _httpClient = httpClient;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var result = await _apiRequestService.PostAsync<LoginRequest, AuthResponse>("api/auth/login", request);

        if (result is not null)
        {
            await SaveAuthDataAsync(result);
        }

        return result;
    }

    public async Task<AuthResponse?> RegisterOwnerAsync(RegisterOwnerRequest request)
    {
        var result = await _apiRequestService.PostAsync<RegisterOwnerRequest, AuthResponse>("api/auth/register-owner", request);

        if (result is not null)
        {
            await SaveAuthDataAsync(result);
        }

        return result;
    }

    public async Task<AuthResponse?> GetCurrentUserAsync()
    {
        return await _apiRequestService.GetAsync<AuthResponse>("api/auth/me");
    }

    public async Task RestoreUserAsync()
    {
        var token = await _localStorageService.GetItemAsync("token");

        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task SaveAuthDataAsync(AuthResponse response)
    {
        await _localStorageService.SetItemAsync("userId", response.UserId);
        await _localStorageService.SetItemAsync("token", response.Token);
        await _localStorageService.SetItemAsync("email", response.Email);
        await _localStorageService.SetItemAsync("fullName", response.FullName);
        await _localStorageService.SetItemAsync("role", response.Role);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);
    }
}
