using System.Security.Claims;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface IAppointmentService
{
    Task<List<AppointmentResponse>> GetAllAsync();
    Task<List<AppointmentResponse>> GetMyAsync(ClaimsPrincipal user);
    Task<List<AppointmentResponse>> GetTodayAsync(ClaimsPrincipal user);
    Task<List<AppointmentResponse>> GetByDoctorAsync(string doctorId, ClaimsPrincipal user);
    Task<AppointmentResponse?> GetByIdAsync(int id, ClaimsPrincipal user);
    Task<AppointmentResponse?> CreateAsync(CreateAppointmentRequest request, ClaimsPrincipal user);
    Task<AppointmentResponse?> UpdateStatusAsync(int id, UpdateAppointmentStatusRequest request, ClaimsPrincipal user);
    Task<bool> DeleteAsync(int id, ClaimsPrincipal user);
}
