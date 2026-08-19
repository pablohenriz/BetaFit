using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages.Orders;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IBetaFitApiClient _api;
    public DetailsModel(IBetaFitApiClient api) => _api = api;
    public OrderDetail? Order { get; private set; }
    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct) { Order = await _api.GetMyOrderAsync(id, ct); return Order is null ? NotFound() : Page(); }
}
