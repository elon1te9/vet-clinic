using Microsoft.EntityFrameworkCore;
using VetClinic.Api.Data;
using VetClinic.Api.Models;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Services;

public class ClinicServiceService
{
    private readonly AppDbContext _context;

    public ClinicServiceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClinicServiceResponse>> GetAllAsync()
    {
        var services = await _context.ClinicServices
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync();

        return services.Select(MapService).ToList();
    }

    public async Task<ClinicServiceResponse?> CreateAsync(CreateClinicServiceRequest request)
    {
        if (!IsValid(request.Name, request.Price, request.DurationMinutes))
        {
            return null;
        }

        var service = new ClinicService
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            Price = request.Price,
            DurationMinutes = request.DurationMinutes,
            IsActive = request.IsActive
        };

        _context.ClinicServices.Add(service);
        await _context.SaveChangesAsync();

        return MapService(service);
    }

    public async Task<ClinicServiceResponse?> UpdateAsync(int id, UpdateClinicServiceRequest request)
    {
        if (!IsValid(request.Name, request.Price, request.DurationMinutes))
        {
            return null;
        }

        var service = await _context.ClinicServices.FirstOrDefaultAsync(s => s.Id == id);
        if (service is null)
        {
            return null;
        }

        service.Name = request.Name.Trim();
        service.Description = request.Description;
        service.Price = request.Price;
        service.DurationMinutes = request.DurationMinutes;
        service.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return MapService(service);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var service = await _context.ClinicServices.FirstOrDefaultAsync(s => s.Id == id);
        if (service is null)
        {
            return false;
        }

        service.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    private static bool IsValid(string name, decimal price, int durationMinutes)
    {
        return !string.IsNullOrWhiteSpace(name) && price >= 0 && durationMinutes > 0;
    }

    private static ClinicServiceResponse MapService(ClinicService service)
    {
        return new ClinicServiceResponse
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            Price = service.Price,
            DurationMinutes = service.DurationMinutes,
            IsActive = service.IsActive
        };
    }
}
