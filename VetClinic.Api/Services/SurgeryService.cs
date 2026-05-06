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

public class SurgeryService : ISurgeryService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public SurgeryService(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<SurgeryResponse>> GetAllAsync(ClaimsPrincipal user)
    {
        var surgeries = await FilterByRole(SurgeriesWithDetails(), user)
            .OrderByDescending(s => s.ScheduledAt)
            .ToListAsync();

        return surgeries.Select(MapSurgery).ToList();
    }

    public async Task<SurgeryResponse?> GetByIdAsync(int id, ClaimsPrincipal user)
    {
        var surgery = await FilterByRole(SurgeriesWithDetails(), user)
            .FirstOrDefaultAsync(s => s.Id == id);

        return surgery is null ? null : MapSurgery(surgery);
    }

    public async Task<List<SurgeryResponse>> GetByPetAsync(int petId, ClaimsPrincipal user)
    {
        var surgeries = await FilterByRole(SurgeriesWithDetails(), user)
            .Where(s => s.PetId == petId)
            .OrderByDescending(s => s.ScheduledAt)
            .ToListAsync();

        return surgeries.Select(MapSurgery).ToList();
    }

    public async Task<SurgeryResponse?> CreateAsync(CreateSurgeryRequest request, ClaimsPrincipal user)
    {
        var veterinarianId = _userManager.GetUserId(user);
        if (!user.IsInRole(nameof(UserRole.Veterinarian)) ||
            string.IsNullOrWhiteSpace(veterinarianId) ||
            !await IsValidRequestAsync(request))
        {
            return null;
        }

        var surgery = new Surgery
        {
            PetId = request.PetId,
            VeterinarianId = veterinarianId,
            AssistantId = request.AssistantId,
            ScheduledAt = ToUtc(request.ScheduledAt),
            Title = request.Title.Trim(),
            Description = request.Description,
            Anesthesia = request.Anesthesia,
            Status = request.Status,
            ResultNotes = request.ResultNotes
        };

        _context.Surgeries.Add(surgery);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(surgery.Id, user);
    }

    public async Task<SurgeryResponse?> UpdateAsync(int id, CreateSurgeryRequest request, ClaimsPrincipal user)
    {
        if (!await IsValidRequestAsync(request))
        {
            return null;
        }

        var surgery = await SurgeriesWithDetails().FirstOrDefaultAsync(s => s.Id == id);
        if (surgery is null || !CanEdit(user, surgery))
        {
            return null;
        }

        surgery.PetId = request.PetId;
        surgery.AssistantId = request.AssistantId;
        surgery.ScheduledAt = ToUtc(request.ScheduledAt);
        surgery.Title = request.Title.Trim();
        surgery.Description = request.Description;
        surgery.Anesthesia = request.Anesthesia;
        surgery.Status = request.Status;
        surgery.ResultNotes = request.ResultNotes;
        surgery.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(surgery.Id, user);
    }

    public async Task<SurgeryResponse?> UpdateStatusAsync(int id, SurgeryStatus status, ClaimsPrincipal user)
    {
        var surgery = await SurgeriesWithDetails().FirstOrDefaultAsync(s => s.Id == id);
        if (surgery is null || !CanEdit(user, surgery))
        {
            return null;
        }

        surgery.Status = status;
        surgery.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(surgery.Id, user);
    }

    private IQueryable<Surgery> SurgeriesWithDetails()
    {
        return _context.Surgeries
            .Include(s => s.Pet)
            .ThenInclude(p => p!.Owner)
            .Include(s => s.Veterinarian)
            .Include(s => s.Assistant);
    }

    private IQueryable<Surgery> FilterByRole(IQueryable<Surgery> query, ClaimsPrincipal user)
    {
        if (user.IsInRole(nameof(UserRole.Admin)) || user.IsInRole(nameof(UserRole.Assistant)))
        {
            return query;
        }

        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return query.Where(s => false);
        }

        if (user.IsInRole(nameof(UserRole.Veterinarian)))
        {
            return query.Where(s => s.VeterinarianId == userId);
        }

        if (user.IsInRole(nameof(UserRole.Owner)))
        {
            return query.Where(s => s.Pet != null && s.Pet.OwnerId == userId);
        }

        return query.Where(s => false);
    }

    private bool CanEdit(ClaimsPrincipal user, Surgery surgery)
    {
        if (user.IsInRole(nameof(UserRole.Admin)))
        {
            return true;
        }

        var userId = _userManager.GetUserId(user);
        return user.IsInRole(nameof(UserRole.Veterinarian)) &&
               !string.IsNullOrWhiteSpace(userId) &&
               surgery.VeterinarianId == userId;
    }

    private async Task<bool> IsValidRequestAsync(CreateSurgeryRequest request)
    {
        if (request.PetId <= 0 ||
            string.IsNullOrWhiteSpace(request.AssistantId) ||
            string.IsNullOrWhiteSpace(request.Title))
        {
            return false;
        }

        var petExists = await _context.Pets.AnyAsync(p => p.Id == request.PetId);
        var assistant = await _userManager.FindByIdAsync(request.AssistantId);

        return petExists &&
               assistant is not null &&
               await _userManager.IsInRoleAsync(assistant, nameof(UserRole.Assistant));
    }

    private static DateTime ToUtc(DateTime value)
    {
        return value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private static SurgeryResponse MapSurgery(Surgery surgery)
    {
        return new SurgeryResponse
        {
            Id = surgery.Id,
            PetId = surgery.PetId,
            PetName = surgery.Pet?.Name ?? string.Empty,
            VeterinarianId = surgery.VeterinarianId,
            VeterinarianFullName = surgery.Veterinarian?.FullName ?? string.Empty,
            AssistantId = surgery.AssistantId,
            AssistantFullName = surgery.Assistant?.FullName ?? string.Empty,
            ScheduledAt = surgery.ScheduledAt,
            Title = surgery.Title,
            Description = surgery.Description,
            Anesthesia = surgery.Anesthesia,
            Status = surgery.Status,
            ResultNotes = surgery.ResultNotes,
            CreatedAt = surgery.CreatedAt,
            UpdatedAt = surgery.UpdatedAt
        };
    }
}
