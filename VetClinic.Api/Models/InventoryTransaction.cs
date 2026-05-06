namespace VetClinic.Api.Models;

public class InventoryTransaction
{
    public int Id { get; set; }
    public int InventoryItemId { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Reason { get; set; }
    public int? RelatedMedicalRecordId { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public InventoryItem? InventoryItem { get; set; }
    public MedicalRecord? RelatedMedicalRecord { get; set; }
    public ApplicationUser? CreatedByUser { get; set; }
}
