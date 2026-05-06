using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Responses;

public class SurgeryResponse
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public string VeterinarianId { get; set; } = string.Empty;
    public string VeterinarianFullName { get; set; } = string.Empty;
    public string AssistantId { get; set; } = string.Empty;
    public string AssistantFullName { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Anesthesia { get; set; }
    public SurgeryStatus Status { get; set; }
    public string? ResultNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
