using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IAdminDashboardService _adminDashboardService;

    public AdminDashboardController(IAdminDashboardService adminDashboardService)
    {
        _adminDashboardService = adminDashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        return Ok(await _adminDashboardService.GetDashboardAsync());
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetToday()
    {
        return Ok(await _adminDashboardService.GetTodayAsync());
    }
}
