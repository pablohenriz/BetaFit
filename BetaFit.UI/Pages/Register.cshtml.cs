using System.Security.Claims;
using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages;

public class RegisterModel : PageModel
{
    private readonly IBetaFitApiClient _api;
    public RegisterModel(IBetaFitApiClient api) => _api = api;

    [BindProperty] public string Name { get; set; } = "";
    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var result = await _api.RegisterAsync(new RegisterRequest { Name = Name, Email = Email, Password = Password }, ct);
        if (result is null) { ErrorMessage = "Não foi possível criar a conta. Verifique os dados e tente outro e-mail."; return Page(); }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, result.Name), new(ClaimTypes.Email, result.Email),
            new(ClaimTypes.Role, result.Role), new("access_token", result.Token)
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        return RedirectToPage("/Index");
    }
}
