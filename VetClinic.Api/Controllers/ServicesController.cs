using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Services;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/services")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly ClinicServiceService _clinicServiceService;

    public ServicesController(ClinicServiceService clinicServiceService)
    {
        _clinicServiceService = clinicServiceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _clinicServiceService.GetAllAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateClinicServiceRequest request)
    {
        var result = await _clinicServiceService.CreateAsync(request);
        return result is null ? BadRequest("Не удалось создать услугу.") : Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateClinicServiceRequest request)
    {
        var result = await _clinicServiceService.UpdateAsync(id, request);
        return result is null ? BadRequest("Не удалось обновить услугу.") : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _clinicServiceService.DeleteAsync(id);
        return result ? NoContent() : BadRequest("Не удалось удалить услугу.");
    }
}
