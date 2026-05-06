using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class FinanceApiService
{
    private readonly ApiRequestService _apiRequestService;

    public FinanceApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<List<InvoiceResponse>> GetAllAsync()
    {
        return await _apiRequestService.GetAsync<List<InvoiceResponse>>("api/invoices") ?? [];
    }

    public async Task<List<InvoiceResponse>> GetMyAsync()
    {
        return await _apiRequestService.GetAsync<List<InvoiceResponse>>("api/invoices/my") ?? [];
    }

    public async Task<InvoiceResponse?> GetByIdAsync(int id)
    {
        return await _apiRequestService.GetAsync<InvoiceResponse>($"api/invoices/{id}");
    }

    public async Task<InvoiceResponse?> CreateAsync(CreateInvoiceRequest request)
    {
        return await _apiRequestService.PostAsync<CreateInvoiceRequest, InvoiceResponse>("api/invoices", request);
    }

    public async Task<InvoiceResponse?> PayAsync(int id)
    {
        return await _apiRequestService.PutAsync<object, InvoiceResponse>($"api/invoices/{id}/pay", new { });
    }
}
