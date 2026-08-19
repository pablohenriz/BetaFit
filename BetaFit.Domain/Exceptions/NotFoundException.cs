namespace BetaFit.Domain.Exceptions;

/// <summary>
/// Lançada quando uma entidade solicitada não existe.
/// A API traduz este tipo em uma resposta HTTP 404 (Not Found).
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, Guid id)
        : base($"{entityName} com Id '{id}' não foi encontrado(a).")
    {
    }
}
