using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Responses;

public class InventoryTransactionResponse
{
    public int Id { get; set; }
    public int InventoryItemId { get; set; }
    public string InventoryItemName { get; set; } = string.Empty;
    public InventoryTransactionType Type { get; set; }
    public decimal Quantity { get; set; }
    public string? Reason { get; set; }
    public int? RelatedMedicalRecordId { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string CreatedByUserFullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
