using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Responses;

public class InvoiceResponse
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string OwnerFullName { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}
