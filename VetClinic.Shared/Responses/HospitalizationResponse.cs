using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Responses;

public class HospitalizationResponse
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string WardNumber { get; set; } = string.Empty;
    public HospitalizationStatus Status { get; set; }
    public string? FeedingSchedule { get; set; }
    public string? ConditionNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
