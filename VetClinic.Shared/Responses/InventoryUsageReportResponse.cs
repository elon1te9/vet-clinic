namespace VetClinic.Shared.Responses;

public class InventoryUsageReportResponse
{
    public int InventoryItemId { get; set; }
    public string InventoryItemName { get; set; } = string.Empty;
    public decimal QuantityUsed { get; set; }
}
