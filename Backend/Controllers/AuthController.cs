using KingOfKings.Backend.DTOs;
using KingOfKings.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingOfKings.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        try
        {
            var response = await _authService.Login(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        try
        {
            var response = await _authService.Register(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // JWT is stateless, so "logout" is mostly a frontend action (clearing token).
        // We can just return Ok here.
        return Ok(new { message = "Logged out successfully" });
    }
}
