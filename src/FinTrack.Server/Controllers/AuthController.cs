using FinTrack.Server.Services.Auth;
using FinTrack.Shared.DTOs.Auth;
using FinTrack.Shared.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var (success, response, errorMessage, errors) = await _authService.RegisterAsync(request);
        if (!success)
        {
            return BadRequest(new ErrorResponse
            {
                Message = errorMessage ?? "Registration failed",
                Errors = errors
            });
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (success, response, errorMessage) = await _authService.LoginAsync(request);
        if (!success)
        {
            return Unauthorized(new ErrorResponse
            {
                Message = errorMessage ?? "Invalid email or password"
            });
        }

        return Ok(response);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var (success, response, errorMessage) = await _authService.RefreshTokenAsync(request);
        if (!success)
        {
            return BadRequest(new ErrorResponse
            {
                Message = errorMessage ?? "Refresh token invalid"
            });
        }

        return Ok(response);
    }
}
