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

public class HospitalizationService : IHospitalizationService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HospitalizationService(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<HospitalizationResponse>> GetAllAsync()
    {
        var hospitalizations = await HospitalizationsWithDetails()
            .OrderByDescending(h => h.StartAt)
            .ToListAsync();

        return hospitalizations.Select(MapHospitalization).ToList();
    }

    public async Task<List<HospitalizationResponse>> GetActiveAsync()
    {
        var hospitalizations = await HospitalizationsWithDetails()
            .Where(h => h.Status == HospitalizationStatus.Active)
            .OrderByDescending(h => h.StartAt)
            .ToListAsync();

        return hospitalizations.Select(MapHospitalization).ToList();
    }

    public async Task<HospitalizationResponse?> GetByIdAsync(int id)
    {
        var hospitalization = await HospitalizationsWithDetails()
            .FirstOrDefaultAsync(h => h.Id == id);

        return hospitalization is null ? null : MapHospitalization(hospitalization);
    }

    public async Task<List<HospitalizationResponse>> GetByPetAsync(int petId)
    {
        var hospitalizations = await HospitalizationsWithDetails()
            .Where(h => h.PetId == petId)
            .OrderByDescending(h => h.StartAt)
            .ToListAsync();

        return hospitalizations.Select(MapHospitalization).ToList();
    }

    public async Task<HospitalizationResponse?> CreateAsync(CreateHospitalizationRequest request)
    {
        if (!await IsValidRequestAsync(request))
        {
            return null;
        }

        if (request.Status == HospitalizationStatus.Active &&
            await HasAnotherActiveHospitalizationAsync(request.PetId, null))
        {
            return null;
        }

        var hospitalization = new Hospitalization
        {
            PetId = request.PetId,
            StartAt = ToUtc(request.StartAt),
            EndAt = request.EndAt.HasValue ? ToUtc(request.EndAt.Value) : null,
            WardNumber = request.WardNumber.Trim(),
            Status = request.Status,
            FeedingSchedule = request.FeedingSchedule,
            ConditionNotes = request.ConditionNotes
        };

        _context.Hospitalizations.Add(hospitalization);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(hospitalization.Id);
    }

    public async Task<HospitalizationResponse?> UpdateAsync(int id, CreateHospitalizationRequest request)
    {
        if (!await IsValidRequestAsync(request))
        {
            return null;
        }

        var hospitalization = await _context.Hospitalizations.FirstOrDefaultAsync(h => h.Id == id);
        if (hospitalization is null)
        {
            return null;
        }

        if (request.Status == HospitalizationStatus.Active &&
            await HasAnotherActiveHospitalizationAsync(request.PetId, id))
        {
            return null;
        }

        hospitalization.PetId = request.PetId;
        hospitalization.StartAt = ToUtc(request.StartAt);
        hospitalization.EndAt = request.EndAt.HasValue ? ToUtc(request.EndAt.Value) : null;
        hospitalization.WardNumber = request.WardNumber.Trim();
        hospitalization.Status = request.Status;
        hospitalization.FeedingSchedule = request.FeedingSchedule;
        hospitalization.ConditionNotes = request.ConditionNotes;
        hospitalization.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(hospitalization.Id);
    }

    public async Task<HospitalizationResponse?> CloseAsync(int id)
    {
        var hospitalization = await _context.Hospitalizations.FirstOrDefaultAsync(h => h.Id == id);
        if (hospitalization is null)
        {
            return null;
        }

        hospitalization.Status = HospitalizationStatus.Completed;
        hospitalization.EndAt = DateTime.UtcNow;
        hospitalization.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(hospitalization.Id);
    }

    public async Task<List<CareLogResponse>> GetCareLogsAsync(int hospitalizationId)
    {
        var logs = await _context.CareLogs
            .AsNoTracking()
            .Include(c => c.Staff)
            .Where(c => c.HospitalizationId == hospitalizationId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return logs.Select(MapCareLog).ToList();
    }

    public async Task<CareLogResponse?> CreateCareLogAsync(CreateCareLogRequest request, ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrWhiteSpace(userId) ||
            (!user.IsInRole(nameof(UserRole.Assistant)) && !user.IsInRole(nameof(UserRole.Veterinarian))) ||
            request.HospitalizationId <= 0 ||
            !await _context.Hospitalizations.AnyAsync(h => h.Id == request.HospitalizationId && h.Status == HospitalizationStatus.Active))
        {
            return null;
        }

        var log = new CareLog
        {
            HospitalizationId = request.HospitalizationId,
            StaffId = userId,
            Temperature = request.Temperature,
            Feeding = request.Feeding,
            Medication = request.Medication,
            Condition = request.Condition,
            Notes = request.Notes
        };

        _context.CareLogs.Add(log);
        await _context.SaveChangesAsync();

        log.Staff = await _userManager.FindByIdAsync(userId);

        return MapCareLog(log);
    }

    private IQueryable<Hospitalization> HospitalizationsWithDetails()
    {
        return _context.Hospitalizations
            .Include(h => h.Pet);
    }

    private async Task<bool> IsValidRequestAsync(CreateHospitalizationRequest request)
    {
        var startAt = ToUtc(request.StartAt);
        var endAt = request.EndAt.HasValue ? ToUtc(request.EndAt.Value) : (DateTime?)null;
        if (endAt.HasValue && endAt.Value < startAt)
        {
            return false;
        }

        return request.PetId > 0 &&
               !string.IsNullOrWhiteSpace(request.WardNumber) &&
               await _context.Pets.AnyAsync(p => p.Id == request.PetId);
    }

    private async Task<bool> HasAnotherActiveHospitalizationAsync(int petId, int? currentHospitalizationId)
    {
        return await _context.Hospitalizations.AnyAsync(h =>
            h.PetId == petId &&
            h.Status == HospitalizationStatus.Active &&
            (!currentHospitalizationId.HasValue || h.Id != currentHospitalizationId.Value));
    }

    private static DateTime ToUtc(DateTime value)
    {
        return value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private static HospitalizationResponse MapHospitalization(Hospitalization hospitalization)
    {
        return new HospitalizationResponse
        {
            Id = hospitalization.Id,
            PetId = hospitalization.PetId,
            PetName = hospitalization.Pet?.Name ?? string.Empty,
            StartAt = hospitalization.StartAt,
            EndAt = hospitalization.EndAt,
            WardNumber = hospitalization.WardNumber,
            Status = hospitalization.Status,
            FeedingSchedule = hospitalization.FeedingSchedule,
            ConditionNotes = hospitalization.ConditionNotes,
            CreatedAt = hospitalization.CreatedAt,
            UpdatedAt = hospitalization.UpdatedAt
        };
    }

    private static CareLogResponse MapCareLog(CareLog log)
    {
        return new CareLogResponse
        {
            Id = log.Id,
            HospitalizationId = log.HospitalizationId,
            StaffId = log.StaffId,
            StaffFullName = log.Staff?.FullName ?? string.Empty,
            CreatedAt = log.CreatedAt,
            Temperature = log.Temperature,
            Feeding = log.Feeding,
            Medication = log.Medication,
            Condition = log.Condition,
            Notes = log.Notes
        };
    }
}
