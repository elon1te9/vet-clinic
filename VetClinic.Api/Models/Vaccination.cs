using VetClinic.Shared.Enums;

namespace VetClinic.Api.Models;

public class Vaccination
{
    public int Id { get; set; }
    public int PetId { get; set; }
    public string VeterinarianId { get; set; } = string.Empty;
    public int? VaccineInventoryItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DoneAt { get; set; }
    public DateTime? NextDueAt { get; set; }
    public VaccinationStatus Status { get; set; } = VaccinationStatus.Completed;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Pet? Pet { get; set; }
    public ApplicationUser? Veterinarian { get; set; }
    public InventoryItem? VaccineInventoryItem { get; set; }
}
