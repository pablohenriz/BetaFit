using BetaFit.UI.Models;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BetaFit.UI.Pages;

public class CatalogModel : PageModel
{
    private readonly IBetaFitApiClient _apiClient;

    public CatalogModel(IBetaFitApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? CategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public Gender? Gender { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortBy { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;

    public PagedResponse<ProductListItemResponse> Result { get; private set; } = new();
    public IReadOnlyList<CategoryResponse> Categories { get; private set; } = Array.Empty<CategoryResponse>();
    public bool ApiUnavailable { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Categories = await _apiClient.GetActiveCategoriesAsync(cancellationToken);

            Result = await _apiClient.SearchProductsAsync(new CatalogQuery
            {
                SearchTerm = SearchTerm,
                CategoryId = CategoryId,
                Gender = Gender,
                SortBy = SortBy,
                Page = Page <= 0 ? 1 : Page
            }, cancellationToken);
        }
        catch (HttpRequestException)
        {
            ApiUnavailable = true;
        }
    }
}
