using BetaFit.Application.DTOs.Category;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Exceptions;
using BetaFit.Domain.Interfaces;
using DomainCategory = BetaFit.Domain.Entities.Category;

namespace BetaFit.Application.Services;

/// <summary>
/// Implementa os casos de uso de Category orquestrando repositórios e regras de domínio.
/// Não conhece EF Core, HTTP ou detalhes de infraestrutura - apenas as abstrações do Domain.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainCategory), id);

        return Map(category);
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(bool onlyActive = false, CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(onlyActive, cancellationToken);
        return categories.Select(Map).ToList();
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (await _categoryRepository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken))
            throw new DomainException($"Já existe uma categoria com o nome '{request.Name}'.");

        var category = new DomainCategory(request.Name, request.Description, request.ImageUrl);

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainCategory), id);

        if (await _categoryRepository.ExistsByNameAsync(request.Name, ignoreId: id, cancellationToken: cancellationToken))
            throw new DomainException($"Já existe uma categoria com o nome '{request.Name}'.");

        category.Update(request.Name, request.Description, request.ImageUrl);

        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainCategory), id);

        category.Activate();
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainCategory), id);

        category.Deactivate();
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainCategory), id);

        if (await _categoryRepository.HasProductsAsync(id, cancellationToken))
            throw new DomainException("Não é possível excluir uma categoria que possui produtos vinculados. Desative-a ou remova os produtos primeiro.");

        category.MarkDeleted();
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static CategoryResponse Map(DomainCategory category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description,
        ImageUrl = category.ImageUrl,
        IsActive = category.IsActive,
        ProductCount = category.Products.Count,
        CreatedAt = category.CreatedAt,
        UpdatedAt = category.UpdatedAt
    };
}
