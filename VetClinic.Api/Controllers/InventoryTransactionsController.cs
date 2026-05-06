using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/inventory-transactions")]
[Authorize(Roles = "Admin,Assistant")]
public class InventoryTransactionsController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryTransactionsController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _inventoryService.GetTransactionsAsync());
    }

    [HttpGet("item/{itemId:int}")]
    public async Task<IActionResult> GetByItem(int itemId)
    {
        return Ok(await _inventoryService.GetTransactionsByItemAsync(itemId));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInventoryTransactionRequest request)
    {
        var result = await _inventoryService.CreateTransactionAsync(request, User);
        return result is null ? BadRequest("Could not create inventory transaction.") : Ok(result);
    }
}
