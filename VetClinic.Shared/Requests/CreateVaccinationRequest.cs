using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Requests;

public class CreateVaccinationRequest
{
    public int PetId { get; set; }
    public int? VaccineInventoryItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DoneAt { get; set; } = DateTime.UtcNow;
    public DateTime? NextDueAt { get; set; }
    public VaccinationStatus Status { get; set; } = VaccinationStatus.Completed;
    public string? Notes { get; set; }
}
