namespace VetClinic.Shared.Requests;

public class CreateCareLogRequest
{
    public int HospitalizationId { get; set; }
    public decimal? Temperature { get; set; }
    public string? Feeding { get; set; }
    public string? Medication { get; set; }
    public string? Condition { get; set; }
    public string? Notes { get; set; }
}
