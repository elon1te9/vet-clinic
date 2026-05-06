using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/hospitalizations")]
[Authorize(Roles = "Admin,Veterinarian,Assistant")]
public class HospitalizationsController : ControllerBase
{
    private readonly IHospitalizationService _hospitalizationService;

    public HospitalizationsController(IHospitalizationService hospitalizationService)
    {
        _hospitalizationService = hospitalizationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _hospitalizationService.GetAllAsync());
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        return Ok(await _hospitalizationService.GetActiveAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _hospitalizationService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("pet/{petId:int}")]
    public async Task<IActionResult> GetByPet(int petId)
    {
        return Ok(await _hospitalizationService.GetByPetAsync(petId));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Veterinarian,Assistant")]
    public async Task<IActionResult> Create(CreateHospitalizationRequest request)
    {
        var result = await _hospitalizationService.CreateAsync(request);
        return result is null ? BadRequest("Could not create hospitalization.") : Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Veterinarian,Assistant")]
    public async Task<IActionResult> Update(int id, CreateHospitalizationRequest request)
    {
        var result = await _hospitalizationService.UpdateAsync(id, request);
        return result is null ? BadRequest("Could not update hospitalization.") : Ok(result);
    }

    [HttpPut("{id:int}/close")]
    [Authorize(Roles = "Admin,Veterinarian,Assistant")]
    public async Task<IActionResult> Close(int id)
    {
        var result = await _hospitalizationService.CloseAsync(id);
        return result is null ? BadRequest("Could not close hospitalization.") : Ok(result);
    }
}
