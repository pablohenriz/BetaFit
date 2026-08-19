using BetaFit.Domain.Entities;
using BetaFit.Domain.Interfaces;
using BetaFit.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly BetaFitDbContext _context;

    public ProductRepository(BetaFitDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<PagedResult<Product>> SearchAsync(ProductQuery query, CancellationToken cancellationToken = default)
    {
        var products = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim();
            products = products.Where(p => p.Name.Contains(term) || p.Description.Contains(term));
        }

        if (query.CategoryId.HasValue)
            products = products.Where(p => p.CategoryId == query.CategoryId.Value);

        if (query.Gender.HasValue)
            products = products.Where(p => p.Gender == query.Gender.Value);

        if (query.IsActive.HasValue)
            products = products.Where(p => p.IsActive == query.IsActive.Value);

        if (query.IsFeatured.HasValue)
            products = products.Where(p => p.IsFeatured == query.IsFeatured.Value);

        products = query.SortBy switch
        {
            "price_asc" => products.OrderBy(p => p.Price),
            "price_desc" => products.OrderByDescending(p => p.Price),
            "newest" => products.OrderByDescending(p => p.CreatedAt),
            _ => products.OrderBy(p => p.Name)
        };

        var totalCount = await products.CountAsync(p => !p.IsDeleted, cancellationToken);

        var items = await products
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<IReadOnlyList<Product>> GetFeaturedAsync(int take = 8, CancellationToken cancellationToken = default)
        => await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsFeatured && p.IsActive && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Product>> GetRelatedAsync(Guid productId, Guid categoryId, int take = 4, CancellationToken cancellationToken = default)
        => await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && p.Id != productId && p.IsActive && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Products.CountAsync(cancellationToken);

    public async Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
        => await _context.Products.CountAsync(p => p.IsActive && !p.IsDeleted, cancellationToken);

    public async Task<int> CountFeaturedAsync(CancellationToken cancellationToken = default)
        => await _context.Products.CountAsync(p => p.IsFeatured && !p.IsDeleted, cancellationToken);

    public async Task<int> CountActiveByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
        => await _context.Products.CountAsync(p => p.CategoryId == categoryId && p.IsActive && !p.IsDeleted, cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        => await _context.Products.AddAsync(product, cancellationToken);

    public void Update(Product product)
        => _context.Products.Update(product);

    public void Remove(Product product)
    {
        product.MarkDeleted();
        _context.Products.Update(product);
    }
}
