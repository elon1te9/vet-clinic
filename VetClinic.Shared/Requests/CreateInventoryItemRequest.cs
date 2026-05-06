using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Requests;

public class CreateInventoryItemRequest
{
    public string Name { get; set; } = string.Empty;
    public InventoryCategory Category { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal MinQuantity { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public decimal Price { get; set; }
    public string? Supplier { get; set; }
}
