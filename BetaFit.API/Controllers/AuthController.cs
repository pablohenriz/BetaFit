using BetaFit.Application.DTOs.Auth;
using BetaFit.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await _auth.RegisterAsync(request, cancellationToken)); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await _auth.LoginAsync(request, cancellationToken)); }
        catch (InvalidOperationException ex) { return Unauthorized(new { message = ex.Message }); }
    }
}
