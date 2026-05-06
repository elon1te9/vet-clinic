using VetClinic.Shared.Enums;

namespace VetClinic.Api.Models;

public class Invoice
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public decimal Amount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    public Appointment? Appointment { get; set; }
    public ApplicationUser? Owner { get; set; }
    public ClinicService? Service { get; set; }
}
