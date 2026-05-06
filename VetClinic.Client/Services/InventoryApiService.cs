using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class InventoryApiService
{
    private readonly ApiRequestService _apiRequestService;

    public InventoryApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<List<InventoryItemResponse>> GetAllAsync()
    {
        return await _apiRequestService.GetAsync<List<InventoryItemResponse>>("api/inventory") ?? [];
    }

    public async Task<List<InventoryItemResponse>> GetLowStockAsync()
    {
        return await _apiRequestService.GetAsync<List<InventoryItemResponse>>("api/inventory/low-stock") ?? [];
    }

    public async Task<List<InventoryItemResponse>> GetExpiringAsync()
    {
        return await _apiRequestService.GetAsync<List<InventoryItemResponse>>("api/inventory/expiring") ?? [];
    }

    public async Task<InventoryItemResponse?> GetByIdAsync(int id)
    {
        return await _apiRequestService.GetAsync<InventoryItemResponse>($"api/inventory/{id}");
    }

    public async Task<InventoryItemResponse?> CreateAsync(CreateInventoryItemRequest request)
    {
        return await _apiRequestService.PostAsync<CreateInventoryItemRequest, InventoryItemResponse>("api/inventory", request);
    }

    public async Task<InventoryItemResponse?> UpdateAsync(int id, UpdateInventoryItemRequest request)
    {
        return await _apiRequestService.PutAsync<UpdateInventoryItemRequest, InventoryItemResponse>($"api/inventory/{id}", request);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _apiRequestService.DeleteAsync($"api/inventory/{id}");
    }

    public async Task<List<InventoryTransactionResponse>> GetTransactionsAsync()
    {
        return await _apiRequestService.GetAsync<List<InventoryTransactionResponse>>("api/inventory-transactions") ?? [];
    }

    public async Task<List<InventoryTransactionResponse>> GetTransactionsByItemAsync(int itemId)
    {
        return await _apiRequestService.GetAsync<List<InventoryTransactionResponse>>($"api/inventory-transactions/item/{itemId}") ?? [];
    }

    public async Task<InventoryTransactionResponse?> CreateTransactionAsync(CreateInventoryTransactionRequest request)
    {
        return await _apiRequestService.PostAsync<CreateInventoryTransactionRequest, InventoryTransactionResponse>("api/inventory-transactions", request);
    }
}
