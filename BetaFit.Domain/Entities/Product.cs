using BetaFit.Domain.Common;
using BetaFit.Domain.Enums;
using BetaFit.Domain.Exceptions;

namespace BetaFit.Domain.Entities;

/// <summary>
/// Representa um produto do catálogo institucional da Beta Fit.
/// O preço é demonstrativo: não existe carrinho, checkout ou pagamento real.
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string? ImageUrl { get; private set; }
    public Gender Gender { get; private set; }
    public bool IsFeatured { get; private set; }
    public bool IsActive { get; private set; }

    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    protected Product()
    {
        // Necessário para o EF Core materializar a entidade.
    }

    public Product(
        string name,
        string description,
        decimal price,
        string? imageUrl,
        Guid categoryId,
        Gender gender)
    {
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        SetCategory(categoryId);
        ImageUrl = imageUrl;
        Gender = gender;
        IsActive = true;
        IsFeatured = false;
    }

    public void Update(
        string name,
        string description,
        decimal price,
        string? imageUrl,
        Guid categoryId,
        Gender gender)
    {
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        SetCategory(categoryId);
        ImageUrl = imageUrl;
        Gender = gender;
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

    public void MarkAsFeatured()
    {
        IsFeatured = true;
        Touch();
    }

    public void UnmarkAsFeatured()
    {
        IsFeatured = false;
        Touch();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do produto é obrigatório.");

        if (name.Length > 150)
            throw new DomainException("O nome do produto deve ter no máximo 150 caracteres.");

        Name = name.Trim();
    }

    private void SetDescription(string description)
    {
        if (description is not null && description.Length > 2000)
            throw new DomainException("A descrição do produto deve ter no máximo 2000 caracteres.");

        Description = description?.Trim() ?? string.Empty;
    }

    private void SetPrice(decimal price)
    {
        if (price < 0)
            throw new DomainException("O preço do produto não pode ser negativo.");

        Price = price;
    }

    private void SetCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new DomainException("O produto deve pertencer a uma categoria válida.");

        CategoryId = categoryId;
    }
}
