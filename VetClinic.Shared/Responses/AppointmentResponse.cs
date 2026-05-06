using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Responses;

public class AppointmentResponse
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string OwnerFullName { get; set; } = string.Empty;
    public string VeterinarianId { get; set; } = string.Empty;
    public string VeterinarianFullName { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal ServicePrice { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string? Reason { get; set; }
    public AppointmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
