using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/care-logs")]
[Authorize(Roles = "Admin,Veterinarian,Assistant")]
public class CareLogsController : ControllerBase
{
    private readonly IHospitalizationService _hospitalizationService;

    public CareLogsController(IHospitalizationService hospitalizationService)
    {
        _hospitalizationService = hospitalizationService;
    }

    [HttpGet("hospitalization/{hospitalizationId:int}")]
    public async Task<IActionResult> GetByHospitalization(int hospitalizationId)
    {
        return Ok(await _hospitalizationService.GetCareLogsAsync(hospitalizationId));
    }

    [HttpPost]
    [Authorize(Roles = "Assistant,Veterinarian")]
    public async Task<IActionResult> Create(CreateCareLogRequest request)
    {
        var result = await _hospitalizationService.CreateCareLogAsync(request, User);
        return result is null ? BadRequest("Could not create care log.") : Ok(result);
    }
}
