using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _userService.GetUsersAsync());
    }

    [HttpGet("users/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("users/{id}/block")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BlockUser(string id)
    {
        var result = await _userService.BlockUserAsync(id);
        return result ? NoContent() : BadRequest("Не удалось заблокировать пользователя.");
    }

    [HttpPut("users/{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(string id, UpdateUserRoleRequest request)
    {
        var result = await _userService.UpdateUserRoleAsync(id, request);
        return result is null ? BadRequest("Не удалось изменить роль пользователя.") : Ok(result);
    }

    [HttpGet("owners")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOwners()
    {
        return Ok(await _userService.GetOwnersAsync());
    }

    [HttpGet("owners/my")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> GetMyOwner()
    {
        var result = await _userService.GetMyOwnerAsync(User);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("owners/{id}")]
    [Authorize(Roles = "Admin,Veterinarian,Assistant")]
    public async Task<IActionResult> GetOwnerById(string id)
    {
        var result = await _userService.GetOwnerByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("staff")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStaff()
    {
        return Ok(await _userService.GetStaffAsync());
    }

    [HttpGet("staff/doctors")]
    public async Task<IActionResult> GetDoctors()
    {
        return Ok(await _userService.GetDoctorsAsync());
    }

    [HttpGet("staff/assistants")]
    [Authorize(Roles = "Admin,Veterinarian")]
    public async Task<IActionResult> GetAssistants()
    {
        return Ok(await _userService.GetAssistantsAsync());
    }

    [HttpGet("staff/{id}")]
    [Authorize(Roles = "Admin,Veterinarian,Assistant")]
    public async Task<IActionResult> GetStaffById(string id)
    {
        var result = await _userService.GetStaffByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}
