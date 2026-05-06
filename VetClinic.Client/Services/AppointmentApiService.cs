using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class AppointmentApiService
{
    private readonly ApiRequestService _apiRequestService;

    public AppointmentApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<List<AppointmentResponse>> GetAllAsync()
    {
        return await _apiRequestService.GetAsync<List<AppointmentResponse>>("api/appointments") ?? [];
    }

    public async Task<List<AppointmentResponse>> GetMyAsync()
    {
        return await _apiRequestService.GetAsync<List<AppointmentResponse>>("api/appointments/my") ?? [];
    }

    public async Task<List<AppointmentResponse>> GetTodayAsync()
    {
        return await _apiRequestService.GetAsync<List<AppointmentResponse>>("api/appointments/today") ?? [];
    }

    public async Task<List<AppointmentResponse>> GetByDoctorAsync(string doctorId)
    {
        return await _apiRequestService.GetAsync<List<AppointmentResponse>>($"api/appointments/doctor/{doctorId}") ?? [];
    }

    public async Task<AppointmentResponse?> GetByIdAsync(int id)
    {
        return await _apiRequestService.GetAsync<AppointmentResponse>($"api/appointments/{id}");
    }

    public async Task<AppointmentResponse?> CreateAsync(CreateAppointmentRequest request)
    {
        return await _apiRequestService.PostAsync<CreateAppointmentRequest, AppointmentResponse>("api/appointments", request);
    }

    public async Task<AppointmentResponse?> UpdateStatusAsync(int id, UpdateAppointmentStatusRequest request)
    {
        return await _apiRequestService.PutAsync<UpdateAppointmentStatusRequest, AppointmentResponse>($"api/appointments/{id}/status", request);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _apiRequestService.DeleteAsync($"api/appointments/{id}");
    }
}
