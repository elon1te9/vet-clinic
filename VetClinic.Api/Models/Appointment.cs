using VetClinic.Shared.Enums;

namespace VetClinic.Api.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string VeterinarianId { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string? Reason { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Planned;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Pet? Pet { get; set; }
    public ApplicationUser? Owner { get; set; }
    public ApplicationUser? Veterinarian { get; set; }
    public ClinicService? Service { get; set; }
    public MedicalRecord? MedicalRecord { get; set; }
    public Invoice? Invoice { get; set; }
}
