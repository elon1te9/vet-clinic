using System.Security.Claims;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface IFinanceService
{
    Task<List<InvoiceResponse>> GetAllAsync();
    Task<List<InvoiceResponse>> GetMyAsync(ClaimsPrincipal user);
    Task<InvoiceResponse?> GetByIdAsync(int id, ClaimsPrincipal user);
    Task<InvoiceResponse?> CreateAsync(CreateInvoiceRequest request);
    Task<InvoiceResponse?> PayAsync(int id);
}
