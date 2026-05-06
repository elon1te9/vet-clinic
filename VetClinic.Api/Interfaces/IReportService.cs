using VetClinic.Shared.Responses;

namespace VetClinic.Api.Interfaces;

public interface IReportService
{
    Task<RevenueReportResponse> GetRevenueAsync(DateTime? from, DateTime? to);
    Task<List<DoctorLoadReportResponse>> GetDoctorLoadAsync(DateTime? from, DateTime? to);
    Task<List<PopularServiceReportResponse>> GetPopularServicesAsync(DateTime? from, DateTime? to);
    Task<List<InventoryUsageReportResponse>> GetInventoryUsageAsync(DateTime? from, DateTime? to);
    Task<List<AppointmentsSummaryReportResponse>> GetAppointmentsSummaryAsync(DateTime? from, DateTime? to);
}
