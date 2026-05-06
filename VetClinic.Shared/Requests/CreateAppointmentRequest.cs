namespace VetClinic.Shared.Requests;

public class CreateAppointmentRequest
{
    public int PetId { get; set; }
    public string VeterinarianId { get; set; } = string.Empty;
    public int ServiceId { get; set; }
    public DateTime StartAt { get; set; }
    public string? Reason { get; set; }
}
