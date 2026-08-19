using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly IBetaFitApiClient _api;
    public ProfileModel(IBetaFitApiClient api) => _api = api;
    public ProfileResponse? Profile { get; private set; }
    public async Task OnGetAsync(CancellationToken ct) => Profile = await _api.GetProfileAsync(ct);
}
