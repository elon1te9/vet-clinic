using Microsoft.AspNetCore.Identity;

namespace VetClinic.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Specialization { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    public ICollection<Appointment> OwnerAppointments { get; set; } = new List<Appointment>();
    public ICollection<Appointment> VeterinarianAppointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
    public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
