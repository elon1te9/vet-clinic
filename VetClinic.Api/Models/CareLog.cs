namespace VetClinic.Api.Models;

public class CareLog
{
    public int Id { get; set; }
    public int HospitalizationId { get; set; }
    public string StaffId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal? Temperature { get; set; }
    public string? Feeding { get; set; }
    public string? Medication { get; set; }
    public string? Condition { get; set; }
    public string? Notes { get; set; }

    public Hospitalization? Hospitalization { get; set; }
    public ApplicationUser? Staff { get; set; }
}
