using System.Security.Claims;
using VetClinic.Shared.Requests;
using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface IInventoryService
{
    Task<List<InventoryItemResponse>> GetAllAsync();
    Task<List<InventoryItemResponse>> GetLowStockAsync();
    Task<List<InventoryItemResponse>> GetExpiringAsync();
    Task<InventoryItemResponse?> GetByIdAsync(int id);
    Task<InventoryItemResponse?> CreateItemAsync(CreateInventoryItemRequest request);
    Task<InventoryItemResponse?> UpdateItemAsync(int id, UpdateInventoryItemRequest request);
    Task<bool> DeleteItemAsync(int id);
    Task<List<InventoryTransactionResponse>> GetTransactionsAsync();
    Task<List<InventoryTransactionResponse>> GetTransactionsByItemAsync(int itemId);
    Task<InventoryTransactionResponse?> CreateTransactionAsync(CreateInventoryTransactionRequest request, ClaimsPrincipal user);
}
