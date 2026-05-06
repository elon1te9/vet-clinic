using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VetClinic.Api.Models;
using VetClinic.Shared.Enums;

namespace VetClinic.Api.Data;

public static class DbInitializer
{
    private static readonly string[] Roles = ["Admin", "Veterinarian", "Owner", "Assistant"];

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = configuration["SeedAdmin:Email"] ?? "admin@vetclinic.local";
        var adminPassword = configuration["SeedAdmin:Password"] ?? "Password123!";
        var adminFullName = configuration["SeedAdmin:FullName"] ?? "Системный администратор";

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = adminFullName,
                IsActive = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(" ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }

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
                    Name = "Vaccine Nobivac",
                    Category = InventoryCategory.Vaccine,
                    Unit = "pcs",
                    Quantity = 10,
                    MinQuantity = 3,
                    ExpirationDate = DateTime.SpecifyKind(DateTime.UtcNow.Date.AddMonths(6), DateTimeKind.Utc),
                    Price = 700,
                    Supplier = "VetSupplier",
                    IsActive = true
                },
                new InventoryItem
                {
                    Name = "Bandage",
                    Category = InventoryCategory.Consumable,
                    Unit = "pcs",
                    Quantity = 25,
                    MinQuantity = 5,
                    Price = 80,
                    Supplier = "MedStore",
                    IsActive = true
                });

            await context.SaveChangesAsync();
        }
    }
}
