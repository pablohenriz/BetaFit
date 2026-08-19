using BetaFit.Domain.Common;
using BetaFit.Domain.Exceptions;

namespace BetaFit.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public User? User { get; private set; }
    public Product? Product { get; private set; }

    protected CartItem() { }

    public CartItem(Guid userId, Guid productId, int quantity = 1)
    {
        if (userId == Guid.Empty || productId == Guid.Empty) throw new DomainException("Usuário e produto são obrigatórios.");
        SetQuantity(quantity);
        UserId = userId;
        ProductId = productId;
    }

    public void Increase(int amount = 1) => SetQuantity(Quantity + amount);
    public void Decrease(int amount = 1) => SetQuantity(Quantity - amount);
    public void SetQuantity(int quantity)
    {
        if (quantity < 1) throw new DomainException("A quantidade deve ser maior que zero.");
        if (quantity > 999) throw new DomainException("A quantidade máxima permitida é 999.");
        Quantity = quantity;
        Touch();
    }
}
