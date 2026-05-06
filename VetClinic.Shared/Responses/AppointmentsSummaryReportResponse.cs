using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Responses;

public class AppointmentsSummaryReportResponse
{
    public AppointmentStatus Status { get; set; }
    public int Count { get; set; }
}
