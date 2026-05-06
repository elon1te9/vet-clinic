namespace VetClinic.Shared.Requests;

public class CreateMedicalRecordRequest
{
    public int? AppointmentId { get; set; }
    public int PetId { get; set; }
    public DateTime VisitDate { get; set; } = DateTime.UtcNow;
    public string? Complaints { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public string? Recommendations { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Weight { get; set; }
}
