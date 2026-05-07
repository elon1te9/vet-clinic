using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Assistant")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _appointmentService.GetAllAsync());
    }

    [HttpGet("my")]
    [Authorize(Roles = "Owner,Veterinarian")]
    public async Task<IActionResult> GetMy()
    {
        return Ok(await _appointmentService.GetMyAsync(User));
    }

    [HttpGet("today")]
    [Authorize(Roles = "Admin,Assistant,Veterinarian")]
    public async Task<IActionResult> GetToday()
    {
        return Ok(await _appointmentService.GetTodayAsync(User));
    }

    [HttpGet("doctor/{doctorId}")]
    [Authorize(Roles = "Admin,Assistant,Veterinarian")]
    public async Task<IActionResult> GetByDoctor(string doctorId)
    {
        return Ok(await _appointmentService.GetByDoctorAsync(doctorId, User));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _appointmentService.GetByIdAsync(id, User);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Owner,Veterinarian")]
    public async Task<IActionResult> Create(CreateAppointmentRequest request)
    {
        var result = await _appointmentService.CreateAsync(request, User);
        return result is null ? BadRequest("Не удалось создать запись. Возможно, время уже занято.") : Ok(result);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin,Veterinarian")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateAppointmentStatusRequest request)
    {
        var result = await _appointmentService.UpdateStatusAsync(id, request, User);
        return result is null ? BadRequest("Не удалось изменить статус записи.") : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _appointmentService.DeleteAsync(id, User);
        return result ? NoContent() : BadRequest("Не удалось отменить запись.");
    }
}
