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

public class VaccinationService : IVaccinationService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;

    public VaccinationService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<List<VaccinationResponse>> GetAllAsync()
    {
        var vaccinations = await VaccinationsWithDetails()
            .OrderByDescending(v => v.DoneAt)
            .ToListAsync();

        return vaccinations.Select(MapVaccination).ToList();
    }

    public async Task<List<VaccinationResponse>> GetMyAsync(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return [];
        }

        var query = VaccinationsWithDetails();

        if (user.IsInRole(nameof(UserRole.Owner)))
        {
            query = query.Where(v => v.Pet != null && v.Pet.OwnerId == userId);
        }
        else if (user.IsInRole(nameof(UserRole.Veterinarian)))
        {
            query = query.Where(v => v.VeterinarianId == userId);
        }
        else
        {
            return [];
        }

        var vaccinations = await query
            .OrderByDescending(v => v.DoneAt)
            .ToListAsync();

        return vaccinations.Select(MapVaccination).ToList();
    }

    public async Task<List<VaccinationResponse>> GetByPetAsync(int petId, ClaimsPrincipal user)
    {
        var pet = await _context.Pets.AsNoTracking().FirstOrDefaultAsync(p => p.Id == petId);
        if (pet is null || !CanAccessPetVaccinations(user, pet))
        {
            return [];
        }

        var vaccinations = await VaccinationsWithDetails()
            .Where(v => v.PetId == petId)
            .OrderByDescending(v => v.DoneAt)
            .ToListAsync();

        return vaccinations.Select(MapVaccination).ToList();
    }

    public async Task<List<VaccinationResponse>> GetUpcomingAsync(ClaimsPrincipal user)
    {
        var today = DateTime.UtcNow.Date;
        var nextMonth = today.AddDays(30);

        var vaccinations = await FilterByRole(VaccinationsWithDetails(), user)
            .Where(v => v.NextDueAt != null &&
                        v.NextDueAt >= today &&
                        v.NextDueAt <= nextMonth &&
                        v.Status != VaccinationStatus.Cancelled)
            .OrderBy(v => v.NextDueAt)
            .ToListAsync();

        return vaccinations.Select(MapVaccination).ToList();
    }

    public async Task<List<VaccinationResponse>> GetOverdueAsync(ClaimsPrincipal user)
    {
        var today = DateTime.UtcNow.Date;

        var vaccinations = await FilterByRole(VaccinationsWithDetails(), user)
            .Where(v => v.NextDueAt != null &&
                        v.NextDueAt < today &&
                        v.Status != VaccinationStatus.Cancelled)
            .OrderBy(v => v.NextDueAt)
            .ToListAsync();

        return vaccinations.Select(MapVaccination).ToList();
    }

    public async Task<VaccinationResponse?> CreateAsync(CreateVaccinationRequest request, ClaimsPrincipal user)
    {
        if (request.PetId <= 0 || string.IsNullOrWhiteSpace(request.Name))
        {
            return null;
        }

        var pet = await _context.Pets
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == request.PetId);

        var veterinarianId = _userManager.GetUserId(user);
        if (pet is null || string.IsNullOrWhiteSpace(veterinarianId))
        {
            return null;
        }

        if (request.VaccineInventoryItemId.HasValue &&
            !await _context.InventoryItems.AnyAsync(i => i.Id == request.VaccineInventoryItemId.Value))
        {
            return null;
        }

        var vaccination = new Vaccination
        {
            PetId = request.PetId,
            VeterinarianId = veterinarianId,
            VaccineInventoryItemId = request.VaccineInventoryItemId,
            Name = request.Name.Trim(),
            DoneAt = ToUtc(request.DoneAt),
            NextDueAt = request.NextDueAt.HasValue ? ToUtc(request.NextDueAt.Value) : null,
            Status = request.Status,
            Notes = request.Notes
        };

        _context.Vaccinations.Add(vaccination);
        await _context.SaveChangesAsync();

        if (vaccination.NextDueAt.HasValue)
        {
            await _notificationService.CreateAsync(
                pet.OwnerId,
                "Напоминание о вакцинации",
                $"Питомцу {pet.Name} запланирована вакцинация {vaccination.Name} на {vaccination.NextDueAt.Value:dd.MM.yyyy}.",
                NotificationType.Vaccination);
        }

        vaccination.Pet = pet;
        vaccination.Veterinarian = await _userManager.FindByIdAsync(veterinarianId);

        return MapVaccination(vaccination);
    }

    public async Task<VaccinationResponse?> UpdateAsync(int id, CreateVaccinationRequest request, ClaimsPrincipal user)
    {
        if (request.PetId <= 0 || string.IsNullOrWhiteSpace(request.Name))
        {
            return null;
        }

        var vaccination = await VaccinationsWithDetails().FirstOrDefaultAsync(v => v.Id == id);
        if (vaccination is null || !CanEditVaccination(user, vaccination))
        {
            return null;
        }

        var pet = await _context.Pets.Include(p => p.Owner).FirstOrDefaultAsync(p => p.Id == request.PetId);
        if (pet is null)
        {
            return null;
        }

        if (request.VaccineInventoryItemId.HasValue &&
            !await _context.InventoryItems.AnyAsync(i => i.Id == request.VaccineInventoryItemId.Value))
        {
            return null;
        }

        vaccination.PetId = request.PetId;
        vaccination.VaccineInventoryItemId = request.VaccineInventoryItemId;
        vaccination.Name = request.Name.Trim();
        vaccination.DoneAt = ToUtc(request.DoneAt);
        vaccination.NextDueAt = request.NextDueAt.HasValue ? ToUtc(request.NextDueAt.Value) : null;
        vaccination.Status = request.Status;
        vaccination.Notes = request.Notes;

        await _context.SaveChangesAsync();

        vaccination.Pet = pet;

        return MapVaccination(vaccination);
    }

    private IQueryable<Vaccination> VaccinationsWithDetails()
    {
        return _context.Vaccinations
            .Include(v => v.Pet)
            .ThenInclude(p => p!.Owner)
            .Include(v => v.Veterinarian)
            .Include(v => v.VaccineInventoryItem);
    }

    private IQueryable<Vaccination> FilterByRole(IQueryable<Vaccination> query, ClaimsPrincipal user)
    {
        if (user.IsInRole(nameof(UserRole.Admin)) || user.IsInRole(nameof(UserRole.Veterinarian)))
        {
            return query;
        }

        var userId = _userManager.GetUserId(user);
        if (user.IsInRole(nameof(UserRole.Owner)) && !string.IsNullOrWhiteSpace(userId))
        {
            return query.Where(v => v.Pet != null && v.Pet.OwnerId == userId);
        }

        return query.Where(v => false);
    }

    private bool CanAccessPetVaccinations(ClaimsPrincipal user, Pet pet)
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

    private bool CanEditVaccination(ClaimsPrincipal user, Vaccination vaccination)
    {
        if (user.IsInRole(nameof(UserRole.Admin)))
        {
            return true;
        }

        var userId = _userManager.GetUserId(user);
        return user.IsInRole(nameof(UserRole.Veterinarian)) &&
               !string.IsNullOrWhiteSpace(userId) &&
               vaccination.VeterinarianId == userId;
    }

    private static DateTime ToUtc(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            return value;
        }

        return DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private static VaccinationResponse MapVaccination(Vaccination vaccination)
    {
        return new VaccinationResponse
        {
            Id = vaccination.Id,
            PetId = vaccination.PetId,
            PetName = vaccination.Pet?.Name ?? string.Empty,
            OwnerId = vaccination.Pet?.OwnerId ?? string.Empty,
            OwnerFullName = vaccination.Pet?.Owner?.FullName ?? string.Empty,
            VeterinarianId = vaccination.VeterinarianId,
            VeterinarianFullName = vaccination.Veterinarian?.FullName ?? string.Empty,
            VaccineInventoryItemId = vaccination.VaccineInventoryItemId,
            VaccineInventoryItemName = vaccination.VaccineInventoryItem?.Name,
            Name = vaccination.Name,
            DoneAt = vaccination.DoneAt,
            NextDueAt = vaccination.NextDueAt,
            Status = vaccination.Status,
            Notes = vaccination.Notes,
            CreatedAt = vaccination.CreatedAt
        };
    }
}
