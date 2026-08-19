using BetaFit.Application.DTOs.Category;

namespace BetaFit.Application.Interfaces;

/// <summary>
/// Casos de uso relacionados a Category. Consumido pelos Controllers da API.
/// </summary>
public interface ICategoryService
{
    Task<CategoryResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(bool onlyActive = false, CancellationToken cancellationToken = default);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
