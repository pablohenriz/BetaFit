using System.Security.Claims;
using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages;

public class LoginModel : PageModel
{
    private readonly IBetaFitApiClient _api;
    public LoginModel(IBetaFitApiClient api) => _api = api;

    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var result = await _api.LoginAsync(new LoginRequest { Email = Email, Password = Password }, ct);
        if (result is null) { ErrorMessage = "E-mail ou senha inválidos."; return Page(); }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, result.Name),
            new(ClaimTypes.Email, result.Email),
            new(ClaimTypes.Role, result.Role),
            new("access_token", result.Token)
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        return RedirectToPage("/Index");
    }
}
