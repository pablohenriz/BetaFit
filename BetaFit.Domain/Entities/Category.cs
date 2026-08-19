using BetaFit.Domain.Common;
using BetaFit.Domain.Exceptions;

namespace BetaFit.Domain.Entities;

/// <summary>
/// Representa uma categoria de produtos (ex: Camisetas, Leggings, Masculino).
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    protected Category()
    {
        // Necessário para o EF Core materializar a entidade.
    }

    public Category(string name, string description, string? imageUrl)
    {
        SetName(name);
        SetDescription(description);
        ImageUrl = imageUrl;
        IsActive = true;
    }

    public void Update(string name, string description, string? imageUrl)
    {
        SetName(name);
        SetDescription(description);
        ImageUrl = imageUrl;
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome da categoria é obrigatório.");

        if (name.Length > 100)
            throw new DomainException("O nome da categoria deve ter no máximo 100 caracteres.");

        Name = name.Trim();
    }

    private void SetDescription(string description)
    {
        if (description is not null && description.Length > 500)
            throw new DomainException("A descrição da categoria deve ter no máximo 500 caracteres.");

        Description = description?.Trim() ?? string.Empty;
    }
}
