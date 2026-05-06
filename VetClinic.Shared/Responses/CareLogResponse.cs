namespace VetClinic.Shared.Responses;

public class CareLogResponse
{
    public int Id { get; set; }
    public int HospitalizationId { get; set; }
    public string StaffId { get; set; } = string.Empty;
    public string StaffFullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal? Temperature { get; set; }
    public string? Feeding { get; set; }
    public string? Medication { get; set; }
    public string? Condition { get; set; }
    public string? Notes { get; set; }
}
