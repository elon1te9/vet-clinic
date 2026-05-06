namespace VetClinic.Shared.Responses;

public class PopularServiceReportResponse
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public int AppointmentsCount { get; set; }
}
