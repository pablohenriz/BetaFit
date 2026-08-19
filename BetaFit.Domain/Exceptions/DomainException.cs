namespace BetaFit.Domain.Exceptions;

/// <summary>
/// Exceção base para violações de regras de negócio dentro do Domain.
/// A API traduz este tipo em uma resposta HTTP 400 (Bad Request).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
