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

public class PetService : IPetService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PetService(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<PetResponse>> GetAllAsync()
    {
        var pets = await _context.Pets
            .AsNoTracking()
            .Include(p => p.Owner)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return pets.Select(MapPet).ToList();
    }

    public async Task<List<PetResponse>> GetMyAsync(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return [];
        }

        var pets = await _context.Pets
            .AsNoTracking()
            .Include(p => p.Owner)
            .Where(p => p.OwnerId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return pets.Select(MapPet).ToList();
    }

    public async Task<PetResponse?> GetByIdAsync(int id, ClaimsPrincipal user)
    {
        var pet = await _context.Pets
            .AsNoTracking()
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pet is null || !CanAccessPet(user, pet))
        {
            return null;
        }

        return MapPet(pet);
    }

    public async Task<PetResponse?> CreateAsync(CreatePetRequest request, ClaimsPrincipal user)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Species))
        {
            return null;
        }

        var ownerId = await ResolveOwnerIdAsync(request.OwnerId, user);
        if (string.IsNullOrWhiteSpace(ownerId))
        {
            return null;
        }

        var owner = await _userManager.FindByIdAsync(ownerId);
        if (owner is null)
        {
            return null;
        }

        var pet = new Pet
        {
            OwnerId = ownerId,
            Name = request.Name.Trim(),
            Species = request.Species.Trim(),
            Breed = request.Breed,
            Gender = request.Gender,
            BirthDate = ToUtc(request.BirthDate),
            Weight = request.Weight,
            Color = request.Color,
            HealthNotes = request.HealthNotes
        };

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        pet.Owner = owner;
        return MapPet(pet);
    }

    public async Task<PetResponse?> UpdateAsync(int id, UpdatePetRequest request, ClaimsPrincipal user)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Species))
        {
            return null;
        }

        var pet = await _context.Pets
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pet is null || !CanEditPet(user, pet))
        {
            return null;
        }

        if (user.IsInRole(nameof(UserRole.Admin)) && !string.IsNullOrWhiteSpace(request.OwnerId))
        {
            var newOwner = await _userManager.FindByIdAsync(request.OwnerId);
            if (newOwner is null)
            {
                return null;
            }

            pet.OwnerId = newOwner.Id;
            pet.Owner = newOwner;
        }

        pet.Name = request.Name.Trim();
        pet.Species = request.Species.Trim();
        pet.Breed = request.Breed;
        pet.Gender = request.Gender;
        pet.BirthDate = ToUtc(request.BirthDate);
        pet.Weight = request.Weight;
        pet.Color = request.Color;
        pet.HealthNotes = request.HealthNotes;

        await _context.SaveChangesAsync();

        return MapPet(pet);
    }

    public async Task<bool> DeleteAsync(int id, ClaimsPrincipal user)
    {
        var pet = await _context.Pets.FirstOrDefaultAsync(p => p.Id == id);

        if (pet is null || !CanEditPet(user, pet))
        {
            return false;
        }

        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string?> ResolveOwnerIdAsync(string? requestedOwnerId, ClaimsPrincipal user)
    {
        if (user.IsInRole(nameof(UserRole.Owner)))
        {
            return _userManager.GetUserId(user);
        }

        if (user.IsInRole(nameof(UserRole.Admin)) && !string.IsNullOrWhiteSpace(requestedOwnerId))
        {
            return requestedOwnerId;
        }

        return null;
    }

    private bool CanAccessPet(ClaimsPrincipal user, Pet pet)
    {
        if (user.IsInRole(nameof(UserRole.Admin)) ||
            user.IsInRole(nameof(UserRole.Veterinarian)) ||
            user.IsInRole(nameof(UserRole.Assistant)))
        {
            return true;
        }

        var userId = _userManager.GetUserId(user);
        return !string.IsNullOrWhiteSpace(userId) && pet.OwnerId == userId;
    }

    private bool CanEditPet(ClaimsPrincipal user, Pet pet)
    {
        if (user.IsInRole(nameof(UserRole.Admin)))
        {
            return true;
        }

        var userId = _userManager.GetUserId(user);
        return user.IsInRole(nameof(UserRole.Owner)) &&
               !string.IsNullOrWhiteSpace(userId) &&
               pet.OwnerId == userId;
    }

    private static DateTime? ToUtc(DateTime? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value.Value.Kind == DateTimeKind.Utc)
        {
            return value;
        }

        return DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
    }

    private static PetResponse MapPet(Pet pet)
    {
        return new PetResponse
        {
            Id = pet.Id,
            OwnerId = pet.OwnerId,
            OwnerFullName = pet.Owner?.FullName ?? string.Empty,
            Name = pet.Name,
            Species = pet.Species,
            Breed = pet.Breed,
            Gender = pet.Gender,
            BirthDate = pet.BirthDate,
            Weight = pet.Weight,
            Color = pet.Color,
            HealthNotes = pet.HealthNotes,
            CreatedAt = pet.CreatedAt
        };
    }
}
