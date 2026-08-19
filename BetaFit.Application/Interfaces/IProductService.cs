using BetaFit.Application.DTOs.Product;

namespace BetaFit.Application.Interfaces;

/// <summary>
/// Casos de uso relacionados a Product. Consumido pelos Controllers da API.
/// </summary>
public interface IProductService
{
    Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<ProductListItemResponse>> SearchAsync(ProductQueryRequest query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductListItemResponse>> GetFeaturedAsync(int take = 8, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductListItemResponse>> GetRelatedAsync(Guid productId, int take = 4, CancellationToken cancellationToken = default);

    Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task SetFeaturedAsync(Guid id, bool isFeatured, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
