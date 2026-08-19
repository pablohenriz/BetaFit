using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages.Admin;

[Authorize(Roles = "Admin")]
public class OrdersModel : PageModel
{
    private readonly IBetaFitApiClient _api;
    public OrdersModel(IBetaFitApiClient api) => _api = api;
    public IReadOnlyList<OrderListItem> Orders { get; private set; } = Array.Empty<OrderListItem>();
    public async Task OnGetAsync(CancellationToken ct) => Orders = await _api.GetAdminOrdersAsync(ct);
}
