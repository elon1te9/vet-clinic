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

public class FinanceService : IFinanceService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public FinanceService(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<InvoiceResponse>> GetAllAsync()
    {
        var invoices = await InvoicesWithDetails()
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return invoices.Select(MapInvoice).ToList();
    }

    public async Task<List<InvoiceResponse>> GetMyAsync(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return [];
        }

        var invoices = await InvoicesWithDetails()
            .Where(i => i.OwnerId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return invoices.Select(MapInvoice).ToList();
    }

    public async Task<InvoiceResponse?> GetByIdAsync(int id, ClaimsPrincipal user)
    {
        var invoice = await InvoicesWithDetails().FirstOrDefaultAsync(i => i.Id == id);
        if (invoice is null)
        {
            return null;
        }

        var userId = _userManager.GetUserId(user);
        if (!user.IsInRole(nameof(UserRole.Admin)) && invoice.OwnerId != userId)
        {
            return null;
        }

        return MapInvoice(invoice);
    }

    public async Task<InvoiceResponse?> CreateAsync(CreateInvoiceRequest request)
    {
        if (request.AppointmentId <= 0 ||
            await _context.Invoices.AnyAsync(i => i.AppointmentId == request.AppointmentId))
        {
            return null;
        }

        var appointment = await _context.Appointments
            .Include(a => a.Service)
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId);

        if (appointment?.Service is null)
        {
            return null;
        }

        var invoice = new Invoice
        {
            AppointmentId = appointment.Id,
            OwnerId = appointment.OwnerId,
            ServiceId = appointment.ServiceId,
            Amount = appointment.Service.Price,
            Status = InvoiceStatus.Unpaid
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        var created = await InvoicesWithDetails().FirstOrDefaultAsync(i => i.Id == invoice.Id);
        return created is null ? null : MapInvoice(created);
    }

    public async Task<InvoiceResponse?> PayAsync(int id)
    {
        var invoice = await InvoicesWithDetails().FirstOrDefaultAsync(i => i.Id == id);
        if (invoice is null || invoice.Status == InvoiceStatus.Cancelled)
        {
            return null;
        }

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapInvoice(invoice);
    }

    private IQueryable<Invoice> InvoicesWithDetails()
    {
        return _context.Invoices
            .Include(i => i.Owner)
            .Include(i => i.Service)
            .Include(i => i.Appointment);
    }

    private static InvoiceResponse MapInvoice(Invoice invoice)
    {
        return new InvoiceResponse
        {
            Id = invoice.Id,
            AppointmentId = invoice.AppointmentId,
            OwnerId = invoice.OwnerId,
            OwnerFullName = invoice.Owner?.FullName ?? string.Empty,
            ServiceId = invoice.ServiceId,
            ServiceName = invoice.Service?.Name ?? string.Empty,
            Amount = invoice.Amount,
            Status = invoice.Status,
            CreatedAt = invoice.CreatedAt,
            PaidAt = invoice.PaidAt
        };
    }
}
