using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages;

[Authorize]
public class CartModel : PageModel
{
    private readonly IBetaFitApiClient _api;
    public CartModel(IBetaFitApiClient api) => _api = api;
    public CartApiResponse Cart { get; private set; } = new();
    public string? Message { get; private set; }

    public async Task OnGetAsync(CancellationToken ct) => await Load(ct);
    public async Task<IActionResult> OnPostUpdateAsync(Guid itemId, int quantity, CancellationToken ct) { await _api.UpdateCartItemAsync(itemId, quantity, ct); return RedirectToPage(); }
    public async Task<IActionResult> OnPostRemoveAsync(Guid itemId, CancellationToken ct) { await _api.RemoveCartItemAsync(itemId, ct); return RedirectToPage(); }
    public async Task<IActionResult> OnPostClearAsync(CancellationToken ct) { await _api.ClearCartAsync(ct); return RedirectToPage(); }
    public async Task<IActionResult> OnPostCheckoutAsync(CancellationToken ct)
    {
        var order = await _api.CreateOrderAsync(ct);
        if (order is null) { Message = "Não foi possível finalizar o pedido. Verifique seu carrinho e tente novamente."; await Load(ct); return Page(); }
        return RedirectToPage("/Orders/Details", new { id = order.Id });
    }
    private async Task Load(CancellationToken ct) { Cart = await _api.GetCartAsync(ct) ?? new(); }
}
