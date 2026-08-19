namespace BetaFit.Application.DTOs.Store;

public class ProfileResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CartItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => Price * Quantity;
}

public class CartResponse
{
    public IReadOnlyList<CartItemResponse> Items { get; set; } = Array.Empty<CartItemResponse>();
    public decimal Total { get; set; }
    public int ItemCount => Items.Sum(x => x.Quantity);
}

public class AddCartItemRequest { public Guid ProductId { get; set; } public int Quantity { get; set; } = 1; }
public class UpdateCartItemRequest { public int Quantity { get; set; } }

public class OrderListItemResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}

public class OrderDetailResponse : OrderListItemResponse
{
    public IReadOnlyList<OrderItemResponse> Items { get; set; } = Array.Empty<OrderItemResponse>();
}

public class OrderItemResponse
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}

public class UpdateOrderStatusRequest { public string Status { get; set; } = string.Empty; }

public class AdminUserResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class AdminDashboardResponse
{
    public int TotalUsers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
}
