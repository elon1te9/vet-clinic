using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetClinic.Api.Interfaces;
using VetClinic.Api.Models;
using VetClinic.Shared.Enums;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserResponse>> GetUsersAsync()
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .OrderBy(u => u.FullName)
            .ToListAsync();

        return await MapUsersAsync(users);
    }

    public async Task<UserResponse?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        return user is null ? null : await MapUserAsync(user);
    }

    public async Task<bool> BlockUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return false;
        }

        user.IsActive = false;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<UserResponse?> UpdateUserRoleAsync(string id, UpdateUserRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return null;
        }

        var newRole = request.Role.ToString();
        if (!await _userManager.IsInRoleAsync(user, newRole))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return null;
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                return null;
            }
        }

        return await MapUserAsync(user);
    }

    public async Task<List<UserResponse>> GetOwnersAsync()
    {
        var owners = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Owner));
        return await MapUsersAsync(owners.OrderBy(o => o.FullName).ToList());
    }

    public async Task<UserResponse?> GetMyOwnerAsync(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        return await GetUserByIdAsync(userId);
    }

    public async Task<UserResponse?> GetOwnerByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null || !await _userManager.IsInRoleAsync(user, nameof(UserRole.Owner)))
        {
            return null;
        }

        return await MapUserAsync(user);
    }

    public async Task<List<UserResponse>> GetStaffAsync()
    {
        var allStaff = new List<ApplicationUser>();
        allStaff.AddRange(await _userManager.GetUsersInRoleAsync(nameof(UserRole.Admin)));
        allStaff.AddRange(await _userManager.GetUsersInRoleAsync(nameof(UserRole.Veterinarian)));
        allStaff.AddRange(await _userManager.GetUsersInRoleAsync(nameof(UserRole.Assistant)));

        return await MapUsersAsync(allStaff
            .GroupBy(u => u.Id)
            .Select(g => g.First())
            .OrderBy(u => u.FullName)
            .ToList());
    }

    public async Task<List<UserResponse>> GetDoctorsAsync()
    {
        var doctors = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Veterinarian));
        return await MapUsersAsync(doctors.OrderBy(d => d.FullName).ToList());
    }

    public async Task<UserResponse?> GetStaffByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(nameof(UserRole.Admin)) &&
            !roles.Contains(nameof(UserRole.Veterinarian)) &&
            !roles.Contains(nameof(UserRole.Assistant)))
        {
            return null;
        }

        return MapUser(user, roles.FirstOrDefault() ?? string.Empty);
    }

    private async Task<List<UserResponse>> MapUsersAsync(IReadOnlyCollection<ApplicationUser> users)
    {
        var result = new List<UserResponse>();

        foreach (var user in users)
        {
            var mapped = await MapUserAsync(user);
            if (mapped is not null)
            {
                result.Add(mapped);
            }
        }

        return result;
    }

    private async Task<UserResponse?> MapUserAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(role))
        {
            return null;
        }

        return MapUser(user, role);
    }

    private static UserResponse MapUser(ApplicationUser user, string role)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Specialization = user.Specialization,
            IsActive = user.IsActive,
            Role = role,
            CreatedAt = user.CreatedAt
        };
    }
}
