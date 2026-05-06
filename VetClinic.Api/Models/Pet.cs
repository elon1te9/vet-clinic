using VetClinic.Shared.Enums;

namespace VetClinic.Api.Models;

public class Pet
{
    public int Id { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public PetGender Gender { get; set; } = PetGender.Unknown;
    public DateTime? BirthDate { get; set; }
    public decimal? Weight { get; set; }
    public string? Color { get; set; }
    public string? HealthNotes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser? Owner { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
}
