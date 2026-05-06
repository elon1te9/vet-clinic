using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Requests;

public class CreatePetRequest
{
    public string? OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public PetGender Gender { get; set; } = PetGender.Unknown;
    public DateTime? BirthDate { get; set; }
    public decimal? Weight { get; set; }
    public string? Color { get; set; }
    public string? HealthNotes { get; set; }
}
