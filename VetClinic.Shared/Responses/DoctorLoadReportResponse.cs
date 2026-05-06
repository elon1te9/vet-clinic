namespace VetClinic.Shared.Responses;

public class DoctorLoadReportResponse
{
    public string VeterinarianId { get; set; } = string.Empty;
    public string VeterinarianFullName { get; set; } = string.Empty;
    public int AppointmentsCount { get; set; }
}
