using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        return Ok(await _reportService.GetRevenueAsync(from, to));
    }

    [HttpGet("doctor-load")]
    public async Task<IActionResult> GetDoctorLoad([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        return Ok(await _reportService.GetDoctorLoadAsync(from, to));
    }

    [HttpGet("popular-services")]
    public async Task<IActionResult> GetPopularServices([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        return Ok(await _reportService.GetPopularServicesAsync(from, to));
    }

    [HttpGet("inventory-usage")]
    public async Task<IActionResult> GetInventoryUsage([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        return Ok(await _reportService.GetInventoryUsageAsync(from, to));
    }

    [HttpGet("appointments-summary")]
    public async Task<IActionResult> GetAppointmentsSummary([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        return Ok(await _reportService.GetAppointmentsSummaryAsync(from, to));
    }
}
