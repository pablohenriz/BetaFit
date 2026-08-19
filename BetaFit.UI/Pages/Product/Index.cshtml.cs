using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages.Product;

public class IndexModel : PageModel
{
    private readonly IBetaFitApiClient _apiClient;
    public IndexModel(IBetaFitApiClient apiClient) => _apiClient = apiClient;
    public ProductResponse? Product { get; private set; }
    public IReadOnlyList<ProductListItemResponse> RelatedProducts { get; private set; } = Array.Empty<ProductListItemResponse>();
    public bool ApiUnavailable { get; private set; }
    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnPostAddToCartAsync(Guid id, CancellationToken ct)
    {
        if (User.Identity?.IsAuthenticated != true)
            return RedirectToPage("/Login", new { returnUrl = $"/Product?id={id}&handler=addToCart" });
        var result = await _apiClient.AddCartItemAsync(id, 1, ct);
        if (result is null) ErrorMessage = "Não foi possível adicionar o produto ao carrinho.";
        return RedirectToPage("/Cart");
    }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        try
        {
            Product = await _apiClient.GetProductByIdAsync(id, ct);
            if (Product is null) return NotFound();
            RelatedProducts = await _apiClient.GetRelatedProductsAsync(id, 4, ct);
            return Page();
        }
        catch (HttpRequestException) { ApiUnavailable = true; return Page(); }
    }
}
