using Microsoft.EntityFrameworkCore;
using VetClinic.Api.Data;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Enums;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RevenueReportResponse> GetRevenueAsync(DateTime? from, DateTime? to)
    {
        var (start, end) = GetPeriod(from, to);

        var paidInvoices = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.Status == InvoiceStatus.Paid &&
                        i.PaidAt != null &&
                        i.PaidAt >= start &&
                        i.PaidAt <= end)
            .ToListAsync();

        return new RevenueReportResponse
        {
            From = start,
            To = end,
            TotalRevenue = paidInvoices.Sum(i => i.Amount),
            PaidInvoicesCount = paidInvoices.Count
        };
    }

    public async Task<List<DoctorLoadReportResponse>> GetDoctorLoadAsync(DateTime? from, DateTime? to)
    {
        var (start, end) = GetPeriod(from, to);

        var appointments = await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Veterinarian)
            .Where(a => a.StartAt >= start && a.StartAt <= end)
            .ToListAsync();

        return appointments
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
            .OrderByDescending(r => r.AppointmentsCount)
            .ToList();
    }

    public async Task<List<PopularServiceReportResponse>> GetPopularServicesAsync(DateTime? from, DateTime? to)
    {
        var (start, end) = GetPeriod(from, to);

        var appointments = await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Service)
            .Where(a => a.StartAt >= start && a.StartAt <= end)
            .ToListAsync();

        return appointments
            .GroupBy(a => new
            {
                a.ServiceId,
                ServiceName = a.Service?.Name ?? string.Empty
            })
            .Select(g => new PopularServiceReportResponse
            {
                ServiceId = g.Key.ServiceId,
                ServiceName = g.Key.ServiceName,
                AppointmentsCount = g.Count()
            })
            .OrderByDescending(r => r.AppointmentsCount)
            .ToList();
    }

    public async Task<List<InventoryUsageReportResponse>> GetInventoryUsageAsync(DateTime? from, DateTime? to)
    {
        var (start, end) = GetPeriod(from, to);

        var transactions = await _context.InventoryTransactions
            .AsNoTracking()
            .Include(t => t.InventoryItem)
            .Where(t => t.CreatedAt >= start &&
                        t.CreatedAt <= end &&
                        (t.Type == InventoryTransactionType.Outgoing ||
                         t.Type == InventoryTransactionType.WriteOff))
            .ToListAsync();

        return transactions
            .GroupBy(t => new
            {
                t.InventoryItemId,
                ItemName = t.InventoryItem?.Name ?? string.Empty
            })
            .Select(g => new InventoryUsageReportResponse
            {
                InventoryItemId = g.Key.InventoryItemId,
                InventoryItemName = g.Key.ItemName,
                QuantityUsed = g.Sum(t => t.Quantity)
            })
            .OrderByDescending(r => r.QuantityUsed)
            .ToList();
    }

    public async Task<List<AppointmentsSummaryReportResponse>> GetAppointmentsSummaryAsync(DateTime? from, DateTime? to)
    {
        var (start, end) = GetPeriod(from, to);

        var appointments = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.StartAt >= start && a.StartAt <= end)
            .ToListAsync();

        return appointments
            .GroupBy(a => a.Status)
            .Select(g => new AppointmentsSummaryReportResponse
            {
                Status = g.Key,
                Count = g.Count()
            })
            .OrderBy(r => r.Status)
            .ToList();
    }

    private static (DateTime Start, DateTime End) GetPeriod(DateTime? from, DateTime? to)
    {
        var start = ToUtc(from ?? DateTime.UtcNow.Date.AddMonths(-1));
        var end = ToUtc(to?.Date.AddDays(1).AddTicks(-1) ?? DateTime.UtcNow);

        return (start, end);
    }

    private static DateTime ToUtc(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            return value;
        }

        return DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}
