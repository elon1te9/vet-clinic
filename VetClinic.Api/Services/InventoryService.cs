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

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;

    public InventoryService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        INotificationService notificationService)
    {
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<List<InventoryItemResponse>> GetAllAsync()
    {
        var items = await _context.InventoryItems
            .AsNoTracking()
            .Where(i => i.IsActive)
            .OrderBy(i => i.Name)
            .ToListAsync();

        return items.Select(MapItem).ToList();
    }

    public async Task<List<InventoryItemResponse>> GetLowStockAsync()
    {
        var items = await _context.InventoryItems
            .AsNoTracking()
            .Where(i => i.IsActive && i.Quantity <= i.MinQuantity)
            .OrderBy(i => i.Quantity)
            .ToListAsync();

        return items.Select(MapItem).ToList();
    }

    public async Task<List<InventoryItemResponse>> GetExpiringAsync()
    {
        var today = DateTime.UtcNow.Date;
        var limit = today.AddDays(30);

        var items = await _context.InventoryItems
            .AsNoTracking()
            .Where(i => i.IsActive &&
                        i.ExpirationDate != null &&
                        i.ExpirationDate >= today &&
                        i.ExpirationDate <= limit)
            .OrderBy(i => i.ExpirationDate)
            .ToListAsync();

        return items.Select(MapItem).ToList();
    }

    public async Task<InventoryItemResponse?> GetByIdAsync(int id)
    {
        var item = await _context.InventoryItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        return item is null ? null : MapItem(item);
    }

    public async Task<InventoryItemResponse?> CreateItemAsync(CreateInventoryItemRequest request)
    {
        if (!IsValidItem(request.Name, request.Unit, request.Quantity, request.MinQuantity, request.Price))
        {
            return null;
        }

        var item = new InventoryItem
        {
            Name = request.Name.Trim(),
            Category = request.Category,
            Unit = request.Unit.Trim(),
            Quantity = request.Quantity,
            MinQuantity = request.MinQuantity,
            ExpirationDate = request.ExpirationDate.HasValue ? ToUtc(request.ExpirationDate.Value) : null,
            Price = request.Price,
            Supplier = request.Supplier
        };

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();

        await NotifyAdminsAboutLowStockAsync(item);

        return MapItem(item);
    }

    public async Task<InventoryItemResponse?> UpdateItemAsync(int id, UpdateInventoryItemRequest request)
    {
        if (!IsValidItem(request.Name, request.Unit, request.Quantity, request.MinQuantity, request.Price))
        {
            return null;
        }

        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == id);
        if (item is null)
        {
            return null;
        }

        item.Name = request.Name.Trim();
        item.Category = request.Category;
        item.Unit = request.Unit.Trim();
        item.Quantity = request.Quantity;
        item.MinQuantity = request.MinQuantity;
        item.ExpirationDate = request.ExpirationDate.HasValue ? ToUtc(request.ExpirationDate.Value) : null;
        item.Price = request.Price;
        item.Supplier = request.Supplier;
        item.IsActive = request.IsActive;
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await NotifyAdminsAboutLowStockAsync(item);

        return MapItem(item);
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == id);
        if (item is null)
        {
            return false;
        }

        item.IsActive = false;
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<InventoryTransactionResponse>> GetTransactionsAsync()
    {
        var transactions = await TransactionsWithDetails()
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return transactions.Select(MapTransaction).ToList();
    }

    public async Task<List<InventoryTransactionResponse>> GetTransactionsByItemAsync(int itemId)
    {
        var transactions = await TransactionsWithDetails()
            .AsNoTracking()
            .Where(t => t.InventoryItemId == itemId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return transactions.Select(MapTransaction).ToList();
    }

    public async Task<InventoryTransactionResponse?> CreateTransactionAsync(CreateInventoryTransactionRequest request, ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (string.IsNullOrWhiteSpace(userId) || request.InventoryItemId <= 0 || request.Quantity <= 0)
        {
            return null;
        }

        var item = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == request.InventoryItemId && i.IsActive);
        if (item is null)
        {
            return null;
        }

        if (request.RelatedMedicalRecordId.HasValue &&
            !await _context.MedicalRecords.AnyAsync(m => m.Id == request.RelatedMedicalRecordId.Value))
        {
            return null;
        }

        if (request.Type == InventoryTransactionType.Incoming)
        {
            item.Quantity += request.Quantity;
        }
        else if (request.Type == InventoryTransactionType.Outgoing || request.Type == InventoryTransactionType.WriteOff)
        {
            if (item.Quantity < request.Quantity)
            {
                return null;
            }

            item.Quantity -= request.Quantity;
        }
        else if (request.Type == InventoryTransactionType.Correction)
        {
            item.Quantity = request.Quantity;
        }
        else
        {
            return null;
        }

        item.UpdatedAt = DateTime.UtcNow;

        var transaction = new InventoryTransaction
        {
            InventoryItemId = item.Id,
            Type = request.Type,
            Quantity = request.Quantity,
            Reason = request.Reason,
            RelatedMedicalRecordId = request.RelatedMedicalRecordId,
            CreatedByUserId = userId
        };

        _context.InventoryTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        transaction.InventoryItem = item;
        transaction.CreatedByUser = await _userManager.FindByIdAsync(userId);

        await NotifyAdminsAboutLowStockAsync(item);

        return MapTransaction(transaction);
    }

    private IQueryable<InventoryTransaction> TransactionsWithDetails()
    {
        return _context.InventoryTransactions
            .Include(t => t.InventoryItem)
            .Include(t => t.CreatedByUser);
    }

    private static bool IsValidItem(string name, string unit, decimal quantity, decimal minQuantity, decimal price)
    {
        return !string.IsNullOrWhiteSpace(name) &&
               !string.IsNullOrWhiteSpace(unit) &&
               quantity >= 0 &&
               minQuantity >= 0 &&
               price >= 0;
    }

    private async Task NotifyAdminsAboutLowStockAsync(InventoryItem item)
    {
        if (!item.IsActive || item.Quantity > item.MinQuantity)
        {
            return;
        }

        var admins = await _userManager.GetUsersInRoleAsync(nameof(UserRole.Admin));
        foreach (var admin in admins)
        {
            await _notificationService.CreateAsync(
                admin.Id,
                "Критический остаток",
                $"Позиция {item.Name}: осталось {item.Quantity} {item.Unit}, минимум {item.MinQuantity}.",
                NotificationType.Inventory,
                "LowStockDetected");
        }
    }

    private static DateTime ToUtc(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            return value;
        }

        return DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private static InventoryItemResponse MapItem(InventoryItem item)
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

    private static InventoryTransactionResponse MapTransaction(InventoryTransaction transaction)
    {
        return new InventoryTransactionResponse
        {
            Id = transaction.Id,
            InventoryItemId = transaction.InventoryItemId,
            InventoryItemName = transaction.InventoryItem?.Name ?? string.Empty,
            Type = transaction.Type,
            Quantity = transaction.Quantity,
            Reason = transaction.Reason,
            RelatedMedicalRecordId = transaction.RelatedMedicalRecordId,
            CreatedByUserId = transaction.CreatedByUserId,
            CreatedByUserFullName = transaction.CreatedByUser?.FullName ?? string.Empty,
            CreatedAt = transaction.CreatedAt
        };
    }
}
