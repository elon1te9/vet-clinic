using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/medical-records")]
[Authorize]
public class MedicalRecordsController : ControllerBase
{
    private readonly IMedicalRecordService _medicalRecordService;

    public MedicalRecordsController(IMedicalRecordService medicalRecordService)
    {
        _medicalRecordService = medicalRecordService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _medicalRecordService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _medicalRecordService.GetByIdAsync(id, User);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("pet/{petId:int}")]
    [Authorize(Roles = "Admin,Veterinarian,Owner")]
    public async Task<IActionResult> GetByPet(int petId)
    {
        return Ok(await _medicalRecordService.GetByPetAsync(petId, User));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Veterinarian")]
    public async Task<IActionResult> Create(CreateMedicalRecordRequest request)
    {
        var result = await _medicalRecordService.CreateAsync(request, User);
        return result is null ? BadRequest("Не удалось создать медицинскую запись.") : Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Veterinarian")]
    public async Task<IActionResult> Update(int id, CreateMedicalRecordRequest request)
    {
        var result = await _medicalRecordService.UpdateAsync(id, request, User);
        return result is null ? BadRequest("Не удалось обновить медицинскую запись.") : Ok(result);
    }
}
