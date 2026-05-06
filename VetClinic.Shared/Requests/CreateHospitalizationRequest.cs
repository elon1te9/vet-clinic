using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Requests;

public class CreateHospitalizationRequest
{
    public int PetId { get; set; }
    public DateTime StartAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndAt { get; set; }
    public string WardNumber { get; set; } = string.Empty;
    public HospitalizationStatus Status { get; set; } = HospitalizationStatus.Active;
    public string? FeedingSchedule { get; set; }
    public string? ConditionNotes { get; set; }
}
