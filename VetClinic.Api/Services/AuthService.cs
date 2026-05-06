using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using VetClinic.Api.Interfaces;
using VetClinic.Api.Models;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Services;

public class AuthService : IAuthService
{
    private static readonly string[] StaffRoles = ["Admin", "Veterinarian", "Assistant"];

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> RegisterOwnerAsync(RegisterOwnerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.FullName))
        {
            return null;
        }

        var email = request.Email.Trim();
        var fullName = request.FullName.Trim();

        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            return null;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address
        };

        return await CreateUserAsync(user, request.Password, "Owner");
    }

    public async Task<AuthResponse?> RegisterStaffAsync(RegisterStaffRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Role))
        {
            return null;
        }

        var role = request.Role.Trim();
        if (!StaffRoles.Contains(role) || !await _roleManager.RoleExistsAsync(role))
        {
            return null;
        }

        var email = request.Email.Trim();
        var fullName = request.FullName.Trim();

        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            return null;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            PhoneNumber = request.PhoneNumber,
            Specialization = request.Specialization
        };

        return await CreateUserAsync(user, request.Password, role);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var email = request.Email.Trim();
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || !user.IsActive || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return null;
        }

        return await CreateAuthResponseAsync(user);
    }

    public async Task<AuthResponse?> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        var userId = _userManager.GetUserId(principal);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        return await CreateAuthResponseAsync(user);
    }

    private async Task<AuthResponse?> CreateUserAsync(ApplicationUser user, string password, string role)
    {
        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            return null;
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            return null;
        }

        return await CreateAuthResponseAsync(user);
    }

    private async Task<AuthResponse?> CreateAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(role))
        {
            return null;
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(GetTokenLifetimeMinutes());

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Role = role,
            Token = GenerateJwtToken(user, role, expiresAt),
            ExpiresAt = expiresAt
        };
    }

    private string GenerateJwtToken(ApplicationUser user, string role, DateTime expiresAt)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtValue("Key")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: GetJwtValue("Issuer"),
            audience: GetJwtValue("Audience"),
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetTokenLifetimeMinutes()
    {
        return _configuration.GetValue("Jwt:ExpiresMinutes", 120);
    }

    private string GetJwtValue(string name)
    {
        return _configuration[$"Jwt:{name}"] ?? string.Empty;
    }
}
