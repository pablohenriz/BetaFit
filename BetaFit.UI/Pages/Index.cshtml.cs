using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages;

public class IndexModel : PageModel
{
    private readonly IBetaFitApiClient _apiClient;

    public IndexModel(IBetaFitApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public IReadOnlyList<ProductListItemResponse> FeaturedProducts { get; private set; } = Array.Empty<ProductListItemResponse>();
    public IReadOnlyList<CategoryResponse> Categories { get; private set; } = Array.Empty<CategoryResponse>();
    public bool ApiUnavailable { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            FeaturedProducts = await _apiClient.GetFeaturedProductsAsync(8, cancellationToken);
            Categories = await _apiClient.GetActiveCategoriesAsync(cancellationToken);
        }
        catch (HttpRequestException)
        {
            ApiUnavailable = true;
        }
    }
}
