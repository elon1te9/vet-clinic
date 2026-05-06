using VetClinic.Shared.Enums;

namespace VetClinic.Api.Models;

public class Surgery
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public string VeterinarianId { get; set; } = string.Empty;
    public string AssistantId { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Anesthesia { get; set; }
    public SurgeryStatus Status { get; set; } = SurgeryStatus.Planned;
    public string? ResultNotes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Pet? Pet { get; set; }
    public ApplicationUser? Veterinarian { get; set; }
    public ApplicationUser? Assistant { get; set; }
}
