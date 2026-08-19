using BetaFit.Domain.Common;
using BetaFit.Domain.Exceptions;

namespace BetaFit.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal Subtotal => UnitPrice * Quantity;
    public Order? Order { get; private set; }

    protected OrderItem() { }

    public OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (productId == Guid.Empty || string.IsNullOrWhiteSpace(productName)) throw new DomainException("Produto inválido.");
        if (unitPrice < 0 || quantity < 1) throw new DomainException("Preço ou quantidade inválidos.");
        ProductId = productId;
        ProductName = productName.Trim();
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}
