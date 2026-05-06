using VetClinic.Shared.Responses;

namespace VetClinic.Client.Services;

public class ReportApiService
{
    private readonly ApiRequestService _apiRequestService;

    public ReportApiService(ApiRequestService apiRequestService)
    {
        _apiRequestService = apiRequestService;
    }

    public async Task<RevenueReportResponse?> GetRevenueAsync(DateTime from, DateTime to)
    {
        return await _apiRequestService.GetAsync<RevenueReportResponse>($"api/reports/revenue{BuildQuery(from, to)}");
    }

    public async Task<List<DoctorLoadReportResponse>> GetDoctorLoadAsync(DateTime from, DateTime to)
    {
        return await _apiRequestService.GetAsync<List<DoctorLoadReportResponse>>($"api/reports/doctor-load{BuildQuery(from, to)}") ?? [];
    }

    public async Task<List<PopularServiceReportResponse>> GetPopularServicesAsync(DateTime from, DateTime to)
    {
        return await _apiRequestService.GetAsync<List<PopularServiceReportResponse>>($"api/reports/popular-services{BuildQuery(from, to)}") ?? [];
    }

    public async Task<List<InventoryUsageReportResponse>> GetInventoryUsageAsync(DateTime from, DateTime to)
    {
        return await _apiRequestService.GetAsync<List<InventoryUsageReportResponse>>($"api/reports/inventory-usage{BuildQuery(from, to)}") ?? [];
    }

    public async Task<List<AppointmentsSummaryReportResponse>> GetAppointmentsSummaryAsync(DateTime from, DateTime to)
    {
        return await _apiRequestService.GetAsync<List<AppointmentsSummaryReportResponse>>($"api/reports/appointments-summary{BuildQuery(from, to)}") ?? [];
    }

    private static string BuildQuery(DateTime from, DateTime to)
    {
        return $"?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";
    }
}
