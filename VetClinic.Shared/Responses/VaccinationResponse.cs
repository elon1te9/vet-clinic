using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Responses;

public class VaccinationResponse
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string OwnerFullName { get; set; } = string.Empty;
    public string VeterinarianId { get; set; } = string.Empty;
    public string VeterinarianFullName { get; set; } = string.Empty;
    public int? VaccineInventoryItemId { get; set; }
    public string? VaccineInventoryItemName { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DoneAt { get; set; }
    public DateTime? NextDueAt { get; set; }
    public VaccinationStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
