namespace VetClinic.Api.Models;

public class MedicalRecord
{
    public int Id { get; set; }
    public int? AppointmentId { get; set; }
    public int PetId { get; set; }
    public string VeterinarianId { get; set; } = string.Empty;
    public DateTime VisitDate { get; set; }
    public string? Complaints { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public string? Recommendations { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? Weight { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Appointment? Appointment { get; set; }
    public Pet? Pet { get; set; }
    public ApplicationUser? Veterinarian { get; set; }
    public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
}
