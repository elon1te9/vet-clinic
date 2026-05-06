namespace VetClinic.Shared.Responses;

public class MedicalRecordResponse
{
    public int Id { get; set; }
    public int? AppointmentId { get; set; }
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string OwnerFullName { get; set; } = string.Empty;
    public string VeterinarianId { get; set; } = string.Empty;
    public string VeterinarianFullName { get; set; } = string.Empty;
    public DateTime VisitDate { get; set; }
    public string? Complaints { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public string? Recommendations { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Weight { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
