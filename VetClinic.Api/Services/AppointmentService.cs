using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetClinic.Api.Data;
using VetClinic.Api.Interfaces;
using VetClinic.Api.Models;
using VetClinic.Shared.Enums;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;

    public AppointmentService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<List<AppointmentResponse>> GetAllAsync()
    {
        var appointments = await AppointmentsWithDetails()
            .OrderByDescending(a => a.StartAt)
            .ToListAsync();

        return appointments.Select(MapAppointment).ToList();
    }

    public async Task<List<AppointmentResponse>> GetMyAsync(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return [];
        }

        var query = AppointmentsWithDetails();

        if (user.IsInRole(nameof(UserRole.Owner)))
        {
            query = query.Where(a => a.OwnerId == userId);
        }
        else if (user.IsInRole(nameof(UserRole.Veterinarian)))
        {
            query = query.Where(a => a.VeterinarianId == userId);
        }
        else
        {
            return [];
        }

        var appointments = await query
            .OrderByDescending(a => a.StartAt)
            .ToListAsync();

        return appointments.Select(MapAppointment).ToList();
    }

    public async Task<List<AppointmentResponse>> GetTodayAsync(ClaimsPrincipal user)
    {
        var start = DateTime.Today.ToUniversalTime();
        var end = DateTime.Today.AddDays(1).ToUniversalTime();
        var userId = _userManager.GetUserId(user);

        var query = AppointmentsWithDetails()
            .Where(a => a.StartAt >= start && a.StartAt < end);

        if (user.IsInRole(nameof(UserRole.Veterinarian)))
        {
            query = query.Where(a => a.VeterinarianId == userId);
        }
        else if (user.IsInRole(nameof(UserRole.Owner)))
        {
            query = query.Where(a => a.OwnerId == userId);
        }

        var appointments = await query
            .OrderBy(a => a.StartAt)
            .ToListAsync();

        return appointments.Select(MapAppointment).ToList();
    }

    public async Task<List<AppointmentResponse>> GetByDoctorAsync(string doctorId, ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (user.IsInRole(nameof(UserRole.Veterinarian)) && doctorId != userId)
        {
            return [];
        }

        var appointments = await AppointmentsWithDetails()
            .Where(a => a.VeterinarianId == doctorId)
            .OrderByDescending(a => a.StartAt)
            .ToListAsync();

        return appointments.Select(MapAppointment).ToList();
    }

    public async Task<AppointmentResponse?> GetByIdAsync(int id, ClaimsPrincipal user)
    {
        var appointment = await AppointmentsWithDetails()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment is null || !CanAccessAppointment(user, appointment))
        {
            return null;
        }

        return MapAppointment(appointment);
    }

    public async Task<AppointmentResponse?> CreateAsync(CreateAppointmentRequest request, ClaimsPrincipal user)
    {
        if (request.PetId <= 0 || request.ServiceId <= 0 || string.IsNullOrWhiteSpace(request.VeterinarianId))
        {
            return null;
        }

        var startAt = ToUtc(request.StartAt);
        var service = await _context.ClinicServices.FirstOrDefaultAsync(s => s.Id == request.ServiceId && s.IsActive);
        var pet = await _context.Pets.Include(p => p.Owner).FirstOrDefaultAsync(p => p.Id == request.PetId);
        var doctor = await _userManager.FindByIdAsync(request.VeterinarianId);

        if (service is null || pet is null || doctor is null)
        {
            return null;
        }

        if (!await _userManager.IsInRoleAsync(doctor, nameof(UserRole.Veterinarian)))
        {
            return null;
        }

        var userId = _userManager.GetUserId(user);
        if (user.IsInRole(nameof(UserRole.Owner)) && pet.OwnerId != userId)
        {
            return null;
        }

        var endAt = startAt.AddMinutes(service.DurationMinutes);
        if (await IsTimeBusyAsync(request.VeterinarianId, startAt, endAt, null))
        {
            return null;
        }

        var appointment = new Appointment
        {
            PetId = pet.Id,
            OwnerId = pet.OwnerId,
            VeterinarianId = doctor.Id,
            ServiceId = service.Id,
            StartAt = startAt,
            EndAt = endAt,
            Reason = request.Reason,
            Status = AppointmentStatus.Planned
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        appointment.Pet = pet;
        appointment.Owner = pet.Owner;
        appointment.Veterinarian = doctor;
        appointment.Service = service;

        await _notificationService.CreateAsync(
            appointment.VeterinarianId,
            "Новая запись на приём",
            $"Питомец {pet.Name} записан на {appointment.StartAt:dd.MM.yyyy HH:mm}.",
            NotificationType.Appointment,
            "AppointmentCreated");

        return MapAppointment(appointment);
    }

    public async Task<AppointmentResponse?> UpdateStatusAsync(int id, UpdateAppointmentStatusRequest request, ClaimsPrincipal user)
    {
        var appointment = await AppointmentsWithDetails().FirstOrDefaultAsync(a => a.Id == id);
        if (appointment is null)
        {
            return null;
        }

        if (user.IsInRole(nameof(UserRole.Veterinarian)))
        {
            var userId = _userManager.GetUserId(user);
            if (appointment.VeterinarianId != userId)
            {
                return null;
            }
        }

        appointment.Status = request.Status;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _notificationService.CreateAsync(
            appointment.OwnerId,
            "Статус приёма изменён",
            $"Статус приёма для питомца {appointment.Pet?.Name ?? string.Empty}: {appointment.Status}.",
            NotificationType.Appointment,
            "AppointmentStatusChanged");

        return MapAppointment(appointment);
    }

    public async Task<bool> DeleteAsync(int id, ClaimsPrincipal user)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        if (appointment is null)
        {
            return false;
        }

        if (user.IsInRole(nameof(UserRole.Owner)))
        {
            var userId = _userManager.GetUserId(user);
            if (appointment.OwnerId != userId)
            {
                return false;
            }
        }

        if (!user.IsInRole(nameof(UserRole.Admin)) && !user.IsInRole(nameof(UserRole.Owner)))
        {
            return false;
        }

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();

        return true;
    }

    private IQueryable<Appointment> AppointmentsWithDetails()
    {
        return _context.Appointments
            .Include(a => a.Pet)
            .Include(a => a.Owner)
            .Include(a => a.Veterinarian)
            .Include(a => a.Service);
    }

    private async Task<bool> IsTimeBusyAsync(string doctorId, DateTime startAt, DateTime endAt, int? ignoredId)
    {
        return await _context.Appointments.AnyAsync(a =>
            a.VeterinarianId == doctorId &&
            a.Id != ignoredId &&
            a.Status != AppointmentStatus.Cancelled &&
            a.StartAt < endAt &&
            a.EndAt > startAt);
    }

    private bool CanAccessAppointment(ClaimsPrincipal user, Appointment appointment)
    {
        if (user.IsInRole(nameof(UserRole.Admin)) || user.IsInRole(nameof(UserRole.Assistant)))
        {
            return true;
        }

        var userId = _userManager.GetUserId(user);

        return !string.IsNullOrWhiteSpace(userId) &&
               (appointment.OwnerId == userId || appointment.VeterinarianId == userId);
    }

    private static DateTime ToUtc(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            return value;
        }

        return DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private static AppointmentResponse MapAppointment(Appointment appointment)
    {
        return new AppointmentResponse
        {
            Id = appointment.Id,
            PetId = appointment.PetId,
            PetName = appointment.Pet?.Name ?? string.Empty,
            OwnerId = appointment.OwnerId,
            OwnerFullName = appointment.Owner?.FullName ?? string.Empty,
            VeterinarianId = appointment.VeterinarianId,
            VeterinarianFullName = appointment.Veterinarian?.FullName ?? string.Empty,
            ServiceId = appointment.ServiceId,
            ServiceName = appointment.Service?.Name ?? string.Empty,
            ServicePrice = appointment.Service?.Price ?? 0,
            StartAt = appointment.StartAt,
            EndAt = appointment.EndAt,
            Reason = appointment.Reason,
            Status = appointment.Status,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt
        };
    }
}
