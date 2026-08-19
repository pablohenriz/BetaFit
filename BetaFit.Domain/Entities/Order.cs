using BetaFit.Domain.Common;
using BetaFit.Domain.Exceptions;

namespace BetaFit.Domain.Entities;

public class Order : BaseEntity
{
    public Guid UserId { get; private set; }
    public decimal Total { get; private set; }
    public string Status { get; private set; } = "Pending";
    public User? User { get; private set; }
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    protected Order() { }

    public Order(Guid userId)
    {
        if (userId == Guid.Empty) throw new DomainException("O pedido deve pertencer a um usuário.");
        UserId = userId;
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (productId == Guid.Empty || string.IsNullOrWhiteSpace(productName)) throw new DomainException("Item de pedido inválido.");
        if (unitPrice < 0 || quantity < 1) throw new DomainException("Preço ou quantidade inválidos.");
        Items.Add(new OrderItem(productId, productName, unitPrice, quantity));
        RecalculateTotal();
    }

    public void SetStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status)) throw new DomainException("Status inválido.");
        Status = status.Trim();
        Touch();
    }

    private void RecalculateTotal() => Total = Items.Sum(x => x.Subtotal);
}
