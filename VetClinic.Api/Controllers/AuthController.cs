using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinic.Api.Interfaces;
using VetClinic.Shared.Requests;

namespace VetClinic.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register-owner")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterOwner(RegisterOwnerRequest request)
    {
        var result = await _authService.RegisterOwnerAsync(request);
        return result is null ? BadRequest("Не удалось выполнить регистрацию.") : Ok(result);
    }

    [HttpPost("register-staff")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterStaff(RegisterStaffRequest request)
    {
        var result = await _authService.RegisterStaffAsync(request);
        return result is null ? BadRequest("Не удалось выполнить регистрацию.") : Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return result is null ? Unauthorized("Неверный email или пароль.") : Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var result = await _authService.GetCurrentUserAsync(User);
        return result is null ? Unauthorized() : Ok(result);
    }
}
