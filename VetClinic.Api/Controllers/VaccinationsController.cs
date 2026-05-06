using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/vaccinations")]
[Authorize]
public class VaccinationsController : ControllerBase
{
    private readonly IVaccinationService _vaccinationService;

    public VaccinationsController(IVaccinationService vaccinationService)
    {
        _vaccinationService = vaccinationService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Veterinarian")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _vaccinationService.GetAllAsync());
    }

    [HttpGet("my")]
    [Authorize(Roles = "Owner,Veterinarian")]
    public async Task<IActionResult> GetMy()
    {
        return Ok(await _vaccinationService.GetMyAsync(User));
    }

    [HttpGet("pet/{petId:int}")]
    [Authorize(Roles = "Admin,Veterinarian,Owner")]
    public async Task<IActionResult> GetByPet(int petId)
    {
        return Ok(await _vaccinationService.GetByPetAsync(petId, User));
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming()
    {
        return Ok(await _vaccinationService.GetUpcomingAsync(User));
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue()
    {
        return Ok(await _vaccinationService.GetOverdueAsync(User));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Veterinarian")]
    public async Task<IActionResult> Create(CreateVaccinationRequest request)
    {
        var result = await _vaccinationService.CreateAsync(request, User);
        return result is null ? BadRequest("Не удалось создать вакцинацию.") : Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Veterinarian")]
    public async Task<IActionResult> Update(int id, CreateVaccinationRequest request)
    {
        var result = await _vaccinationService.UpdateAsync(id, request, User);
        return result is null ? BadRequest("Не удалось обновить вакцинацию.") : Ok(result);
    }
}
