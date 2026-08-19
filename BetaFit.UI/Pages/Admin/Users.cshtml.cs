using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages.Admin;

[Authorize(Roles = "Admin")]
public class UsersModel : PageModel
{
    private readonly IBetaFitApiClient _api;
    public UsersModel(IBetaFitApiClient api) => _api = api;
    public IReadOnlyList<AdminUser> Users { get; private set; } = Array.Empty<AdminUser>();
    public async Task OnGetAsync(CancellationToken ct) => Users = await _api.GetAdminUsersAsync(ct);
}
