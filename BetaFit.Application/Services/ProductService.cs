using BetaFit.Application.DTOs.Product;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Exceptions;
using BetaFit.Domain.Interfaces;
using DomainProduct = BetaFit.Domain.Entities.Product;

namespace BetaFit.Application.Services;

/// <summary>
/// Implementa os casos de uso de Product orquestrando repositórios e regras de domínio.
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainProduct), id);

        return MapToResponse(product);
    }

    public async Task<PagedResponse<ProductListItemResponse>> SearchAsync(ProductQueryRequest query, CancellationToken cancellationToken = default)
    {
        var domainQuery = new ProductQuery
        {
            SearchTerm = query.SearchTerm,
            CategoryId = query.CategoryId,
            Gender = query.Gender,
            IsActive = query.IsActive,
            IsFeatured = query.IsFeatured,
            SortBy = query.SortBy,
            Page = query.Page <= 0 ? 1 : query.Page,
            PageSize = query.PageSize is <= 0 or > 100 ? 12 : query.PageSize
        };

        var result = await _productRepository.SearchAsync(domainQuery, cancellationToken);

        return new PagedResponse<ProductListItemResponse>
        {
            Items = result.Items.Select(MapToListItem).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };
    }

    public async Task<IReadOnlyList<ProductListItemResponse>> GetFeaturedAsync(int take = 8, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetFeaturedAsync(take, cancellationToken);
        return products.Select(MapToListItem).ToList();
    }

    public async Task<IReadOnlyList<ProductListItemResponse>> GetRelatedAsync(Guid productId, int take = 4, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainProduct), productId);

        var related = await _productRepository.GetRelatedAsync(productId, product.CategoryId, take, cancellationToken);
        return related.Select(MapToListItem).ToList();
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new DomainException("A categoria informada não existe.");

        var product = new DomainProduct(
            request.Name,
            request.Description,
            request.Price,
            request.ImageUrl,
            request.CategoryId,
            request.Gender);

        if (request.IsFeatured)
            product.MarkAsFeatured();

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(product, category.Name);
    }

    public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainProduct), id);

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new DomainException("A categoria informada não existe.");

        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.ImageUrl,
            request.CategoryId,
            request.Gender);

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(product, category.Name);
    }

    public async Task ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainProduct), id);

        product.Activate();
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainProduct), id);

        product.Deactivate();
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task SetFeaturedAsync(Guid id, bool isFeatured, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainProduct), id);

        if (isFeatured)
            product.MarkAsFeatured();
        else
            product.UnmarkAsFeatured();

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainProduct), id);

        _productRepository.Remove(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static ProductResponse MapToResponse(DomainProduct product, string? categoryNameOverride = null) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        ImageUrl = product.ImageUrl,
        CategoryId = product.CategoryId,
        CategoryName = categoryNameOverride ?? product.Category?.Name ?? string.Empty,
        Gender = product.Gender,
        IsFeatured = product.IsFeatured,
        IsActive = product.IsActive,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt
    };

    private static ProductListItemResponse MapToListItem(DomainProduct product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        ImageUrl = product.ImageUrl,
        CategoryName = product.Category?.Name ?? string.Empty,
        Gender = product.Gender,
        IsFeatured = product.IsFeatured,
        IsActive = product.IsActive
    };
}
