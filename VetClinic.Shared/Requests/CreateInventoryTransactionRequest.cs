using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Requests;

public class CreateInventoryTransactionRequest
{
    public int InventoryItemId { get; set; }
    public InventoryTransactionType Type { get; set; }
    public decimal Quantity { get; set; }
    public string? Reason { get; set; }
    public int? RelatedMedicalRecordId { get; set; }
}
