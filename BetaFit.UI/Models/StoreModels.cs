namespace BetaFit.UI.Models;
public class ProfileResponse { public Guid Id { get; set; } public string Name { get; set; } = ""; public string Email { get; set; } = ""; public DateTime CreatedAt { get; set; } public string Role { get; set; } = ""; public bool IsActive { get; set; } }
public class CartApiItem { public Guid Id { get; set; } public Guid ProductId { get; set; } public string Name { get; set; } = ""; public string? ImageUrl { get; set; } public decimal Price { get; set; } public int Quantity { get; set; } public decimal Subtotal { get; set; } }
public class CartApiResponse { public IReadOnlyList<CartApiItem> Items { get; set; } = Array.Empty<CartApiItem>(); public decimal Total { get; set; } public int ItemCount { get; set; } }
public class AddCartItemRequest { public Guid ProductId { get; set; } public int Quantity { get; set; } = 1; }
public class UpdateCartItemRequest { public int Quantity { get; set; } }
public class OrderListItem { public Guid Id { get; set; } public Guid UserId { get; set; } public string UserName { get; set; } = ""; public DateTime CreatedAt { get; set; } public decimal Total { get; set; } public string Status { get; set; } = ""; public int ItemCount { get; set; } }
public class OrderItemApi { public Guid ProductId { get; set; } public string ProductName { get; set; } = ""; public decimal UnitPrice { get; set; } public int Quantity { get; set; } public decimal Subtotal { get; set; } }
public class OrderDetail : OrderListItem { public IReadOnlyList<OrderItemApi> Items { get; set; } = Array.Empty<OrderItemApi>(); }
public class UpdateOrderStatusRequest { public string Status { get; set; } = ""; }
public class AdminUser { public Guid Id { get; set; } public string Name { get; set; } = ""; public string Email { get; set; } = ""; public DateTime CreatedAt { get; set; } public string Role { get; set; } = ""; public bool IsActive { get; set; } }
public class AdminDashboard { public int TotalUsers { get; set; } public int TotalProducts { get; set; } public int TotalOrders { get; set; } }
public class UpdateProductRequest { public string Name { get; set; } = ""; public string Description { get; set; } = ""; public decimal Price { get; set; } public string? ImageUrl { get; set; } public Guid CategoryId { get; set; } public Gender Gender { get; set; } }
