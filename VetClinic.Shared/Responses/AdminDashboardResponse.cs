namespace VetClinic.Shared.Responses;

public class AdminDashboardResponse
{
    public int AppointmentsTodayCount { get; set; }
    public decimal RevenueToday { get; set; }
    public int ActiveHospitalizationsCount { get; set; }
    public int LowStockItemsCount { get; set; }
    public int UpcomingVaccinationsCount { get; set; }
    public List<DoctorLoadReportResponse> DoctorsLoad { get; set; } = [];
    public List<AppointmentResponse> RecentAppointments { get; set; } = [];
    public List<InventoryItemResponse> LowStockItems { get; set; } = [];
    public List<VaccinationResponse> UpcomingVaccinations { get; set; } = [];
}
