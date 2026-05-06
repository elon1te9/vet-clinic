using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize(Roles = "Admin,Veterinarian,Assistant")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _inventoryService.GetAllAsync());
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        return Ok(await _inventoryService.GetLowStockAsync());
    }

    [HttpGet("expiring")]
    public async Task<IActionResult> GetExpiring()
    {
        return Ok(await _inventoryService.GetExpiringAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _inventoryService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateInventoryItemRequest request)
    {
        var result = await _inventoryService.CreateItemAsync(request);
        return result is null ? BadRequest("Could not create inventory item.") : Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateInventoryItemRequest request)
    {
        var result = await _inventoryService.UpdateItemAsync(id, request);
        return result is null ? BadRequest("Could not update inventory item.") : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _inventoryService.DeleteItemAsync(id);
        return result ? NoContent() : BadRequest("Could not delete inventory item.");
    }
}
