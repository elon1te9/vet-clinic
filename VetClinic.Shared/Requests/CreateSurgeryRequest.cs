using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Requests;

public class CreateSurgeryRequest
{
    public int PetId { get; set; }
    public string AssistantId { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; } = DateTime.UtcNow;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Anesthesia { get; set; }
    public SurgeryStatus Status { get; set; } = SurgeryStatus.Planned;
    public string? ResultNotes { get; set; }
}
