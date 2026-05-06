using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Enums;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/surgeries")]
[Authorize(Roles = "Admin,Veterinarian,Assistant,Owner")]
public class SurgeriesController : ControllerBase
{
    private readonly ISurgeryService _surgeryService;

    public SurgeriesController(ISurgeryService surgeryService)
    {
        _surgeryService = surgeryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _surgeryService.GetAllAsync(User));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _surgeryService.GetByIdAsync(id, User);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("pet/{petId:int}")]
    public async Task<IActionResult> GetByPet(int petId)
    {
        return Ok(await _surgeryService.GetByPetAsync(petId, User));
    }

    [HttpPost]
    [Authorize(Roles = "Veterinarian")]
    public async Task<IActionResult> Create(CreateSurgeryRequest request)
    {
        var result = await _surgeryService.CreateAsync(request, User);
        return result is null ? BadRequest("Could not create surgery.") : Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Veterinarian")]
    public async Task<IActionResult> Update(int id, CreateSurgeryRequest request)
    {
        var result = await _surgeryService.UpdateAsync(id, request, User);
        return result is null ? BadRequest("Could not update surgery.") : Ok(result);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin,Veterinarian")]
    public async Task<IActionResult> UpdateStatus(int id, SurgeryStatus status)
    {
        var result = await _surgeryService.UpdateStatusAsync(id, status, User);
        return result is null ? BadRequest("Could not update surgery status.") : Ok(result);
    }
}
