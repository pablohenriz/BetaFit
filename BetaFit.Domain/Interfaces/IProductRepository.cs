using BetaFit.Domain.Entities;
using BetaFit.Domain.Enums;

namespace BetaFit.Domain.Interfaces;

/// <summary>
/// Parâmetros de consulta para o catálogo público e para o Desktop administrativo.
/// Mantido no Domain por ser um conceito simples e sem dependências externas.
/// </summary>
public class ProductQuery
{
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public Gender? Gender { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsFeatured { get; set; }
    public string? SortBy { get; set; } // "name", "price_asc", "price_desc", "newest"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

/// <summary>
/// Contrato de persistência para Product. Implementado pela Infrastructure.
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Product>> SearchAsync(ProductQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetFeaturedAsync(int take = 8, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetRelatedAsync(Guid productId, Guid categoryId, int take = 4, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
    Task<int> CountFeaturedAsync(CancellationToken cancellationToken = default);
    Task<int> CountActiveByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);
    void Remove(Product product);
}
