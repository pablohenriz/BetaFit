using BetaFit.Domain.Common;
using BetaFit.Domain.Exceptions;

namespace BetaFit.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = "Customer";
    public bool IsActive { get; private set; } = true;

    protected User() { }

    public User(string name, string email, string passwordHash, string role = "Customer")
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("E-mail é obrigatório.");
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainException("Senha inválida.");

        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role is "Admin" or "Customer" ? role : "Customer";
    }
}
