using BetaFit.Application.DTOs.Dashboard;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Interfaces;

namespace BetaFit.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;

    public DashboardService(IProductRepository products, ICategoryRepository categories)
    {
        _products = products;
        _categories = categories;
    }

    public async Task<DashboardSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var totalProductsTask = _products.CountAsync(cancellationToken);
        var activeProductsTask = _products.CountActiveAsync(cancellationToken);
        var featuredProductsTask = _products.CountFeaturedAsync(cancellationToken);
        var totalCategoriesTask = _categories.CountAsync(cancellationToken);

        await Task.WhenAll(totalProductsTask, activeProductsTask, featuredProductsTask, totalCategoriesTask);
        var activeCategories = (await _categories.GetAllAsync(true, cancellationToken)).Count;

        return new DashboardSummaryResponse
        {
            TotalProducts = totalProductsTask.Result,
            ActiveProducts = activeProductsTask.Result,
            FeaturedProducts = featuredProductsTask.Result,
            TotalCategories = totalCategoriesTask.Result,
            ActiveCategories = activeCategories
        };
    }
}
