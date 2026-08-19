using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages.Product;

public class IndexModel : PageModel
{
    private readonly IBetaFitApiClient _apiClient;

    public IndexModel(IBetaFitApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public ProductResponse? Product { get; private set; }
    public IReadOnlyList<ProductListItemResponse> RelatedProducts { get; private set; } = Array.Empty<ProductListItemResponse>();
    public bool ApiUnavailable { get; private set; }

    public async Task<IActionResult> OnPostAddToCartAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _apiClient.GetProductByIdAsync(id, cancellationToken);
        if (product is not null) CartService.Add(HttpContext, product);
        return RedirectToPage("/Cart");
    }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            Product = await _apiClient.GetProductByIdAsync(id, cancellationToken);

            if (Product is null)
                return NotFound();

            RelatedProducts = await _apiClient.GetRelatedProductsAsync(id, 4, cancellationToken);
            return Page();
        }
        catch (HttpRequestException)
        {
            ApiUnavailable = true;
            return Page();
        }
    }
}
