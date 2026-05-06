using VetClinic.Shared.Enums;

namespace VetClinic.Api.Models;

public class Hospitalization
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string WardNumber { get; set; } = string.Empty;
    public HospitalizationStatus Status { get; set; } = HospitalizationStatus.Active;
    public string? FeedingSchedule { get; set; }
    public string? ConditionNotes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Pet? Pet { get; set; }
    public ICollection<CareLog> CareLogs { get; set; } = new List<CareLog>();
}
