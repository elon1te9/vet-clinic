using VetClinic.Shared.Enums;

namespace VetClinic.Shared.Requests;

public class UpdateAppointmentStatusRequest
{
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Planned;
}
