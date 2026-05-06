using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Responses;

public class PetResponse
{
    public int Id { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string OwnerFullName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public PetGender Gender { get; set; } = PetGender.Unknown;
    public DateTime? BirthDate { get; set; }
    public decimal? Weight { get; set; }
    public string? Color { get; set; }
    public string? HealthNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}
