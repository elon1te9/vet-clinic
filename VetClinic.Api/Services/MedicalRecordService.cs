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

public class MedicalRecordService : IMedicalRecordService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MedicalRecordService(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<MedicalRecordResponse>> GetAllAsync()
    {
        var records = await RecordsWithDetails()
            .OrderByDescending(r => r.VisitDate)
            .ToListAsync();

        return records.Select(MapRecord).ToList();
    }

    public async Task<MedicalRecordResponse?> GetByIdAsync(int id, ClaimsPrincipal user)
    {
        var record = await RecordsWithDetails()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (record is null || !CanAccessRecord(user, record))
        {
            return null;
        }

        return MapRecord(record);
    }

    public async Task<List<MedicalRecordResponse>> GetByPetAsync(int petId, ClaimsPrincipal user)
    {
        var pet = await _context.Pets.AsNoTracking().FirstOrDefaultAsync(p => p.Id == petId);
        if (pet is null || !CanAccessPetRecords(user, pet))
        {
            return [];
        }

        var records = await RecordsWithDetails()
            .Where(r => r.PetId == petId)
            .OrderByDescending(r => r.VisitDate)
            .ToListAsync();

        return records.Select(MapRecord).ToList();
    }

    public async Task<MedicalRecordResponse?> CreateAsync(CreateMedicalRecordRequest request, ClaimsPrincipal user)
    {
        if (request.PetId <= 0 || !request.AppointmentId.HasValue)
        {
            return null;
        }

        var pet = await _context.Pets
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == request.PetId);

        if (pet is null)
        {
            return null;
        }

        var appointment = await ResolveAppointmentAsync(request.AppointmentId, request.PetId, user);
        if (appointment is null)
        {
            return null;
        }

        if (await _context.MedicalRecords.AnyAsync(r => r.AppointmentId == request.AppointmentId.Value))
        {
            return null;
        }

        var veterinarianId = ResolveVeterinarianId(user, appointment);
        if (string.IsNullOrWhiteSpace(veterinarianId))
        {
            return null;
        }

        var veterinarian = await _userManager.FindByIdAsync(veterinarianId);
        if (veterinarian is null)
        {
            return null;
        }

        var record = new MedicalRecord
        {
            AppointmentId = request.AppointmentId,
            PetId = request.PetId,
            VeterinarianId = veterinarianId,
            VisitDate = ToUtc(request.VisitDate),
            Complaints = request.Complaints,
            Diagnosis = request.Diagnosis,
            Treatment = request.Treatment,
            Recommendations = request.Recommendations,
            Temperature = request.Temperature,
            Weight = request.Weight
        };

        _context.MedicalRecords.Add(record);
        await _context.SaveChangesAsync();

        record.Pet = pet;
        record.Veterinarian = veterinarian;
        record.Appointment = appointment;

        return MapRecord(record);
    }

    public async Task<MedicalRecordResponse?> UpdateAsync(int id, CreateMedicalRecordRequest request, ClaimsPrincipal user)
    {
        if (request.PetId <= 0 || !request.AppointmentId.HasValue)
        {
            return null;
        }

        var record = await RecordsWithDetails().FirstOrDefaultAsync(r => r.Id == id);
        if (record is null || !CanEditRecord(user, record))
        {
            return null;
        }

        if (user.IsInRole(nameof(UserRole.Veterinarian)) && request.PetId != record.PetId)
        {
            return null;
        }

        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == request.PetId);
        if (pet is null)
        {
            return null;
        }

        var appointment = await ResolveAppointmentAsync(request.AppointmentId, request.PetId, user);
        if (appointment is null)
        {
            return null;
        }

        if (await _context.MedicalRecords.AnyAsync(r => r.Id != id && r.AppointmentId == request.AppointmentId.Value))
        {
            return null;
        }

        record.AppointmentId = request.AppointmentId;
        record.PetId = request.PetId;
        record.VeterinarianId = appointment.VeterinarianId;
        record.VisitDate = ToUtc(request.VisitDate);
        record.Complaints = request.Complaints;
        record.Diagnosis = request.Diagnosis;
        record.Treatment = request.Treatment;
        record.Recommendations = request.Recommendations;
        record.Temperature = request.Temperature;
        record.Weight = request.Weight;
        record.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        record.Pet = pet;
        record.Appointment = appointment;
        record.Veterinarian = await _userManager.FindByIdAsync(record.VeterinarianId);

        return MapRecord(record);
    }

    private IQueryable<MedicalRecord> RecordsWithDetails()
    {
        return _context.MedicalRecords
            .Include(r => r.Pet)
            .ThenInclude(p => p!.Owner)
            .Include(r => r.Veterinarian)
            .Include(r => r.Appointment);
    }

    private async Task<Appointment?> ResolveAppointmentAsync(int? appointmentId, int petId, ClaimsPrincipal user)
    {
        if (!appointmentId.HasValue)
        {
            return null;
        }

        var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId.Value);
        if (appointment is null || appointment.PetId != petId)
        {
            return null;
        }

        var userId = _userManager.GetUserId(user);
        if (user.IsInRole(nameof(UserRole.Veterinarian)) && appointment.VeterinarianId != userId)
        {
            return null;
        }

        return appointment;
    }

    private string? ResolveVeterinarianId(ClaimsPrincipal user, Appointment? appointment)
    {
        if (user.IsInRole(nameof(UserRole.Veterinarian)))
        {
            return _userManager.GetUserId(user);
        }

        if (user.IsInRole(nameof(UserRole.Admin)) && appointment is not null)
        {
            return appointment.VeterinarianId;
        }

        return null;
    }

    private bool CanAccessRecord(ClaimsPrincipal user, MedicalRecord record)
    {
        if (user.IsInRole(nameof(UserRole.Admin)) || user.IsInRole(nameof(UserRole.Veterinarian)))
        {
            return true;
        }

        var userId = _userManager.GetUserId(user);
        return user.IsInRole(nameof(UserRole.Owner)) &&
               !string.IsNullOrWhiteSpace(userId) &&
               record.Pet?.OwnerId == userId;
    }

    private bool CanAccessPetRecords(ClaimsPrincipal user, Pet pet)
    {
        if (user.IsInRole(nameof(UserRole.Admin)) || user.IsInRole(nameof(UserRole.Veterinarian)))
        {
            return true;
        }

        var userId = _userManager.GetUserId(user);
        return user.IsInRole(nameof(UserRole.Owner)) &&
               !string.IsNullOrWhiteSpace(userId) &&
               pet.OwnerId == userId;
    }

    private bool CanEditRecord(ClaimsPrincipal user, MedicalRecord record)
    {
        if (user.IsInRole(nameof(UserRole.Admin)))
        {
            return true;
        }

        var userId = _userManager.GetUserId(user);
        return user.IsInRole(nameof(UserRole.Veterinarian)) &&
               !string.IsNullOrWhiteSpace(userId) &&
               record.VeterinarianId == userId;
    }

    private static DateTime ToUtc(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            return value;
        }

        return DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private static MedicalRecordResponse MapRecord(MedicalRecord record)
    {
        return new MedicalRecordResponse
        {
            Id = record.Id,
            AppointmentId = record.AppointmentId,
            PetId = record.PetId,
            PetName = record.Pet?.Name ?? string.Empty,
            OwnerId = record.Pet?.OwnerId ?? string.Empty,
            OwnerFullName = record.Pet?.Owner?.FullName ?? string.Empty,
            VeterinarianId = record.VeterinarianId,
            VeterinarianFullName = record.Veterinarian?.FullName ?? string.Empty,
            VisitDate = record.VisitDate,
            Complaints = record.Complaints,
            Diagnosis = record.Diagnosis,
            Treatment = record.Treatment,
            Recommendations = record.Recommendations,
            Temperature = record.Temperature,
            Weight = record.Weight,
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt
        };
    }
}
