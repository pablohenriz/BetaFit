using System.Security.Claims;
using BetaFit.Application.DTOs.Store;
using BetaFit.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/store")]
public class StoreController : ControllerBase
{
    private readonly StoreService _store;
    public StoreController(StoreService store) => _store = store;
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("profile")]
    public async Task<IActionResult> Profile(CancellationToken ct) => Ok(await _store.GetProfileAsync(UserId, ct));

    [HttpGet("cart")]
    public async Task<IActionResult> Cart(CancellationToken ct) => Ok(await _store.GetCartAsync(UserId, ct));

    [HttpPost("cart/items")]
    public async Task<IActionResult> AddCart(AddCartItemRequest request, CancellationToken ct) => Ok(await _store.AddCartItemAsync(UserId, request, ct));

    [HttpPut("cart/items/{itemId:guid}")]
    public async Task<IActionResult> UpdateCart(Guid itemId, UpdateCartItemRequest request, CancellationToken ct) => Ok(await _store.UpdateCartItemAsync(UserId, itemId, request, ct));

    [HttpDelete("cart/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveCart(Guid itemId, CancellationToken ct) => Ok(await _store.RemoveCartItemAsync(UserId, itemId, ct));

    [HttpDelete("cart")]
    public async Task<IActionResult> ClearCart(CancellationToken ct) { await _store.ClearCartAsync(UserId, ct); return NoContent(); }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder(CancellationToken ct) => Ok(await _store.CreateOrderAsync(UserId, ct));

    [HttpGet("orders")]
    public async Task<IActionResult> MyOrders(CancellationToken ct) => Ok(await _store.GetMyOrdersAsync(UserId, ct));

    [HttpGet("orders/{id:guid}")]
    public async Task<IActionResult> MyOrder(Guid id, CancellationToken ct) => Ok(await _store.GetOrderAsync(UserId, id, ct));
}
