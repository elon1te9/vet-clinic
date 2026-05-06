using Microsoft.EntityFrameworkCore;
using VetClinic.Api.Data;
using VetClinic.Api.Interfaces;
using VetClinic.Api.Models;
using VetClinic.Shared.Enums;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Services;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly AppDbContext _context;

    public AdminDashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardResponse> GetDashboardAsync()
    {
        return await BuildDashboardAsync();
    }

    public async Task<AdminDashboardResponse> GetTodayAsync()
    {
        return await BuildDashboardAsync();
    }

    private async Task<AdminDashboardResponse> BuildDashboardAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var nextMonth = today.AddDays(30);

        var appointmentsToday = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.StartAt >= today && a.StartAt < tomorrow)
            .ToListAsync();

        var paidInvoicesToday = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.Status == InvoiceStatus.Paid &&
                        i.PaidAt != null &&
                        i.PaidAt >= today &&
                        i.PaidAt < tomorrow)
            .ToListAsync();

        var activeHospitalizationsCount = await _context.Hospitalizations
            .AsNoTracking()
            .CountAsync(h => h.Status == HospitalizationStatus.Active);

        var lowStockItems = await _context.InventoryItems
            .AsNoTracking()
            .Where(i => i.IsActive && i.Quantity <= i.MinQuantity)
            .OrderBy(i => i.Quantity)
            .ToListAsync();

        var upcomingVaccinations = await _context.Vaccinations
            .AsNoTracking()
            .Include(v => v.Pet)
            .ThenInclude(p => p!.Owner)
            .Include(v => v.Veterinarian)
            .Include(v => v.VaccineInventoryItem)
            .Where(v => v.NextDueAt != null &&
                        v.NextDueAt >= today &&
                        v.NextDueAt <= nextMonth &&
                        v.Status != VaccinationStatus.Cancelled)
            .OrderBy(v => v.NextDueAt)
            .Take(5)
            .ToListAsync();

        var recentAppointments = await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Pet)
            .Include(a => a.Owner)
            .Include(a => a.Veterinarian)
            .Include(a => a.Service)
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .ToListAsync();

        var doctorsLoad = await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Veterinarian)
            .Where(a => a.StartAt >= today && a.StartAt < tomorrow)
            .ToListAsync();

        return new AdminDashboardResponse
        {
            AppointmentsTodayCount = appointmentsToday.Count,
            RevenueToday = paidInvoicesToday.Sum(i => i.Amount),
            ActiveHospitalizationsCount = activeHospitalizationsCount,
            LowStockItemsCount = lowStockItems.Count,
            UpcomingVaccinationsCount = upcomingVaccinations.Count,
            DoctorsLoad = doctorsLoad
                .GroupBy(a => new
                {
                    a.VeterinarianId,
                    FullName = a.Veterinarian?.FullName ?? string.Empty
                })
                .Select(g => new DoctorLoadReportResponse
                {
                    VeterinarianId = g.Key.VeterinarianId,
                    VeterinarianFullName = g.Key.FullName,
                    AppointmentsCount = g.Count()
                })
                .OrderByDescending(d => d.AppointmentsCount)
                .ToList(),
            RecentAppointments = recentAppointments.Select(MapAppointment).ToList(),
            LowStockItems = lowStockItems.Select(MapInventoryItem).ToList(),
            UpcomingVaccinations = upcomingVaccinations.Select(MapVaccination).ToList()
        };
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

    private static InventoryItemResponse MapInventoryItem(InventoryItem item)
    {
        return new InventoryItemResponse
        {
            Id = item.Id,
            Name = item.Name,
            Category = item.Category,
            Unit = item.Unit,
            Quantity = item.Quantity,
            MinQuantity = item.MinQuantity,
            ExpirationDate = item.ExpirationDate,
            Price = item.Price,
            Supplier = item.Supplier,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
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
