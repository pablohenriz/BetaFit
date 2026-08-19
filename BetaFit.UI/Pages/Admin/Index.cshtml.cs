using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IBetaFitApiClient _api;
    public IndexModel(IBetaFitApiClient api) => _api = api;
    public IReadOnlyList<ProductListItemResponse> Products { get; private set; } = Array.Empty<ProductListItemResponse>();
    public string? Message { get; private set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        try { Products = await _api.GetAdminProductsAsync(ct); }
        catch { Message = "Não foi possível carregar os produtos."; }
    }
}
