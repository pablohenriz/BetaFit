using BetaFit.Domain.Entities;

namespace BetaFit.Domain.Interfaces;

/// <summary>
/// Contrato de persistência para Category. Implementado pela Infrastructure.
/// O Domain não conhece EF Core nem SQL Server - apenas esta abstração.
/// </summary>
public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetAllAsync(bool onlyActive = false, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, Guid? ignoreId = null, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    void Update(Category category);
    void Remove(Category category);
}
