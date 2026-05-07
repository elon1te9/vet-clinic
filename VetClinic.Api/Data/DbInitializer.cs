using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetClinic.Api.Models;
using VetClinic.Shared.Enums;

namespace VetClinic.Api.Data;

public static class DbInitializer
{
    private static readonly string[] Roles =
    [
        nameof(UserRole.Admin),
        nameof(UserRole.Veterinarian),
        nameof(UserRole.Owner),
        nameof(UserRole.Assistant)
    ];

    private const string TestPassword = "Password123!";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await EnsureTestUserAsync(
            userManager,
            "admin@vetclinic.local",
            "Администратор клиники",
            nameof(UserRole.Admin),
            phoneNumber: "+7 900 000-00-01");

        await EnsureTestUserAsync(
            userManager,
            "doctor@vetclinic.local",
            "Петров Пётр Сергеевич",
            nameof(UserRole.Veterinarian),
            phoneNumber: "+7 900 000-00-02",
            specialization: "Терапевт");

        await EnsureTestUserAsync(
            userManager,
            "owner@vetclinic.local",
            "Иванов Иван Иванович",
            nameof(UserRole.Owner),
            phoneNumber: "+7 900 000-00-03",
            address: "Москва, ул. Лесная, 10");

        await EnsureTestUserAsync(
            userManager,
            "assistant@vetclinic.local",
            "Смирнова Анна Викторовна",
            nameof(UserRole.Assistant),
            phoneNumber: "+7 900 000-00-04",
            specialization: "Ассистент");

        if (!await context.ClinicServices.AnyAsync())
        {
            context.ClinicServices.AddRange(
                new ClinicService
                {
                    Name = "Первичный приём",
                    Description = "Осмотр питомца и консультация врача",
                    Price = 1200,
                    DurationMinutes = 30,
                    IsActive = true
                },
                new ClinicService
                {
                    Name = "Повторный приём",
                    Description = "Контроль состояния после лечения",
                    Price = 800,
                    DurationMinutes = 20,
                    IsActive = true
                },
                new ClinicService
                {
                    Name = "Вакцинация",
                    Description = "Осмотр и введение вакцины",
                    Price = 1500,
                    DurationMinutes = 25,
                    IsActive = true
                });

            await context.SaveChangesAsync();
        }

        if (!await context.InventoryItems.AnyAsync())
        {
            context.InventoryItems.AddRange(
                new InventoryItem
                {
                    Name = "Вакцина Нобивак",
                    Category = InventoryCategory.Vaccine,
                    Unit = "шт.",
                    Quantity = 10,
                    MinQuantity = 3,
                    ExpirationDate = DateTime.SpecifyKind(DateTime.UtcNow.Date.AddMonths(6), DateTimeKind.Utc),
                    Price = 700,
                    Supplier = "ВетСнаб",
                    IsActive = true
                },
                new InventoryItem
                {
                    Name = "Бинт стерильный",
                    Category = InventoryCategory.Material,
                    Unit = "шт.",
                    Quantity = 25,
                    MinQuantity = 5,
                    Price = 80,
                    Supplier = "МедСклад",
                    IsActive = true
                });

            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureTestUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string fullName,
        string role,
        string? phoneNumber = null,
        string? specialization = null,
        string? address = null)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                Specialization = specialization,
                Address = address,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(user, TestPassword);
            ThrowIfFailed(createResult);
        }

        user.UserName = email;
        user.Email = email;
        user.EmailConfirmed = true;
        user.FullName = fullName;
        user.PhoneNumber = phoneNumber;
        user.Specialization = specialization;
        user.Address = address;
        user.IsActive = true;

        ThrowIfFailed(await userManager.UpdateAsync(user));

        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToRemove = currentRoles.Where(currentRole => currentRole != role).ToArray();
        if (rolesToRemove.Length > 0)
        {
            ThrowIfFailed(await userManager.RemoveFromRolesAsync(user, rolesToRemove));
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            ThrowIfFailed(await userManager.AddToRoleAsync(user, role));
        }

        if (!await userManager.CheckPasswordAsync(user, TestPassword))
        {
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            ThrowIfFailed(await userManager.ResetPasswordAsync(user, resetToken, TestPassword));
        }
    }

    private static void ThrowIfFailed(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(e => e.Description)));
        }
    }
}
