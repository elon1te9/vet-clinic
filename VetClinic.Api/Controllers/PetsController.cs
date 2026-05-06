using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Enums;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetsController : ControllerBase
{
    private readonly IPetService _petService;

    public PetsController(IPetService petService)
    {
        _petService = petService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Veterinarian,Assistant")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _petService.GetAllAsync());
    }

    [HttpGet("my")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> GetMy()
    {
        return Ok(await _petService.GetMyAsync(User));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _petService.GetByIdAsync(id, User);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Create(CreatePetRequest request)
    {
        var result = await _petService.CreateAsync(request, User);
        return result is null ? BadRequest("Не удалось создать питомца.") : Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Update(int id, UpdatePetRequest request)
    {
        var result = await _petService.UpdateAsync(id, request, User);
        return result is null ? BadRequest("Не удалось обновить питомца.") : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _petService.DeleteAsync(id, User);
        return result ? NoContent() : BadRequest("Не удалось удалить питомца.");
    }
}
