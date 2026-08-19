namespace BetaFit.Domain.Interfaces;

/// <summary>
/// Garante que as operações de escrita feitas através dos repositórios
/// sejam persistidas de forma atômica (um único SaveChanges do EF Core).
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
