using BetaFit.Application.DTOs.Store;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Exceptions;
using BetaFit.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BetaFit.Application.Services;

public class StoreService
{
    private readonly BetaFitDbContext _db;
    public StoreService(BetaFitDbContext db) => _db = db;

    public async Task<ProfileResponse> GetProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, ct)
            ?? throw new NotFoundException(nameof(User), userId);
        return new ProfileResponse { Id = user.Id, Name = user.Name, Email = user.Email, CreatedAt = user.CreatedAt, Role = user.Role, IsActive = user.IsActive };
    }

    public async Task<CartResponse> GetCartAsync(Guid userId, CancellationToken ct = default)
    {
        var items = await _db.Set<CartItem>().AsNoTracking().Include(x => x.Product).Where(x => x.UserId == userId).ToListAsync(ct);
        return MapCart(items);
    }

    public async Task<CartResponse> AddCartItemAsync(Guid userId, AddCartItemRequest request, CancellationToken ct = default)
    {
        if (request.Quantity < 1) throw new DomainException("A quantidade deve ser maior que zero.");
        var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId, ct)
            ?? throw new NotFoundException(nameof(Product), request.ProductId);
        if (!product.IsActive) throw new DomainException("Produto indisponível.");

        var item = await _db.Set<CartItem>().FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == request.ProductId, ct);
        if (item is null) await _db.Set<CartItem>().AddAsync(new CartItem(userId, product.Id, request.Quantity), ct);
        else item.Increase(request.Quantity);
        await _db.SaveChangesAsync(ct);
        return await GetCartAsync(userId, ct);
    }

    public async Task<CartResponse> UpdateCartItemAsync(Guid userId, Guid itemId, UpdateCartItemRequest request, CancellationToken ct = default)
    {
        var item = await _db.Set<CartItem>().FirstOrDefaultAsync(x => x.Id == itemId && x.UserId == userId, ct)
            ?? throw new NotFoundException(nameof(CartItem), itemId);
        item.SetQuantity(request.Quantity);
        await _db.SaveChangesAsync(ct);
        return await GetCartAsync(userId, ct);
    }

    public async Task<CartResponse> RemoveCartItemAsync(Guid userId, Guid itemId, CancellationToken ct = default)
    {
        var item = await _db.Set<CartItem>().FirstOrDefaultAsync(x => x.Id == itemId && x.UserId == userId, ct)
            ?? throw new NotFoundException(nameof(CartItem), itemId);
        _db.Remove(item);
        await _db.SaveChangesAsync(ct);
        return await GetCartAsync(userId, ct);
    }

    public async Task ClearCartAsync(Guid userId, CancellationToken ct = default)
    {
        var items = await _db.Set<CartItem>().Where(x => x.UserId == userId).ToListAsync(ct);
        _db.RemoveRange(items);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<OrderDetailResponse> CreateOrderAsync(Guid userId, CancellationToken ct = default)
    {
        var items = await _db.Set<CartItem>().Include(x => x.Product).Where(x => x.UserId == userId).ToListAsync(ct);
        if (items.Count == 0) throw new DomainException("Seu carrinho está vazio.");
        if (items.Any(x => x.Product is null || !x.Product.IsActive)) throw new DomainException("Um ou mais produtos do carrinho estão indisponíveis.");

        var order = new Order(userId);
        foreach (var item in items)
            order.AddItem(item.ProductId, item.Product!.Name, item.Product.Price, item.Quantity);
        order.SetStatus("Pending");
        await _db.Set<Order>().AddAsync(order, ct);
        _db.RemoveRange(items);
        await _db.SaveChangesAsync(ct);
        return await GetOrderAsync(userId, order.Id, ct);
    }

    public async Task<IReadOnlyList<OrderListItemResponse>> GetMyOrdersAsync(Guid userId, CancellationToken ct = default)
    {
        return await _db.Set<Order>().AsNoTracking().Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt)
            .Select(x => new OrderListItemResponse { Id = x.Id, UserId = x.UserId, UserName = x.User!.Name, CreatedAt = x.CreatedAt, Total = x.Total, Status = x.Status, ItemCount = x.Items.Sum(i => i.Quantity) }).ToListAsync(ct);
    }

    public async Task<OrderDetailResponse> GetOrderAsync(Guid userId, Guid orderId, CancellationToken ct = default)
    {
        var order = await _db.Set<Order>().AsNoTracking().Include(x => x.User).Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == userId, ct)
            ?? throw new NotFoundException(nameof(Order), orderId);
        return MapOrder(order);
    }

    public async Task<IReadOnlyList<OrderListItemResponse>> GetAllOrdersAsync(CancellationToken ct = default)
    {
        return await _db.Set<Order>().AsNoTracking().Include(x => x.User).Include(x => x.Items).OrderByDescending(x => x.CreatedAt)
            .Select(x => new OrderListItemResponse { Id = x.Id, UserId = x.UserId, UserName = x.User!.Name, CreatedAt = x.CreatedAt, Total = x.Total, Status = x.Status, ItemCount = x.Items.Sum(i => i.Quantity) }).ToListAsync(ct);
    }

    public async Task<OrderDetailResponse> GetAdminOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _db.Set<Order>().AsNoTracking().Include(x => x.User).Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == orderId, ct)
            ?? throw new NotFoundException(nameof(Order), orderId);
        return MapOrder(order);
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, string status, CancellationToken ct = default)
    {
        var order = await _db.Set<Order>().FirstOrDefaultAsync(x => x.Id == orderId, ct) ?? throw new NotFoundException(nameof(Order), orderId);
        order.SetStatus(status);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<AdminDashboardResponse> GetAdminDashboardAsync(CancellationToken ct = default) => new()
    {
        TotalUsers = await _db.Users.CountAsync(ct),
        TotalProducts = await _db.Products.CountAsync(ct),
        TotalOrders = await _db.Set<Order>().CountAsync(ct)
    };

    public async Task<IReadOnlyList<AdminUserResponse>> GetUsersAsync(CancellationToken ct = default) =>
        await _db.Users.AsNoTracking().OrderByDescending(x => x.CreatedAt).Select(x => new AdminUserResponse { Id = x.Id, Name = x.Name, Email = x.Email, CreatedAt = x.CreatedAt, Role = x.Role, IsActive = x.IsActive }).ToListAsync(ct);

    private static CartResponse MapCart(List<CartItem> items) => new() { Items = items.Select(x => new CartItemResponse { Id = x.Id, ProductId = x.ProductId, Name = x.Product?.Name ?? "Produto", ImageUrl = x.Product?.ImageUrl, Price = x.Product?.Price ?? 0, Quantity = x.Quantity }).ToList(), Total = items.Sum(x => (x.Product?.Price ?? 0) * x.Quantity) };
    private static OrderDetailResponse MapOrder(Order x) => new() { Id = x.Id, UserId = x.UserId, UserName = x.User?.Name ?? string.Empty, CreatedAt = x.CreatedAt, Total = x.Total, Status = x.Status, ItemCount = x.Items.Sum(i => i.Quantity), Items = x.Items.Select(i => new OrderItemResponse { ProductId = i.ProductId, ProductName = i.ProductName, UnitPrice = i.UnitPrice, Quantity = i.Quantity, Subtotal = i.Subtotal }).ToList() };
}
