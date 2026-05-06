using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/invoices")]
[Authorize]
public class FinanceController : ControllerBase
{
    private readonly IFinanceService _financeService;

    public FinanceController(IFinanceService financeService)
    {
        _financeService = financeService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _financeService.GetAllAsync());
    }

    [HttpGet("my")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> GetMy()
    {
        return Ok(await _financeService.GetMyAsync(User));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _financeService.GetByIdAsync(id, User);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateInvoiceRequest request)
    {
        var result = await _financeService.CreateAsync(request);
        return result is null ? BadRequest("Could not create invoice.") : Ok(result);
    }

    [HttpPut("{id:int}/pay")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Pay(int id)
    {
        var result = await _financeService.PayAsync(id);
        return result is null ? BadRequest("Could not pay invoice.") : Ok(result);
    }
}
