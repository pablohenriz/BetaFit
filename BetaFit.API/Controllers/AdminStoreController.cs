using BetaFit.Application.DTOs.Store;
using BetaFit.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetaFit.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin")]
public class AdminStoreController : ControllerBase
{
    private readonly StoreService _store;
    public AdminStoreController(StoreService store) => _store = store;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken ct) => Ok(await _store.GetAdminDashboardAsync(ct));

    [HttpGet("users")]
    public async Task<IActionResult> Users(CancellationToken ct) => Ok(await _store.GetUsersAsync(ct));

    [HttpGet("orders")]
    public async Task<IActionResult> Orders(CancellationToken ct) => Ok(await _store.GetAllOrdersAsync(ct));

    [HttpGet("orders/{id:guid}")]
    public async Task<IActionResult> Order(Guid id, CancellationToken ct) => Ok(await _store.GetAdminOrderAsync(id, ct));

    [HttpPatch("orders/{id:guid}/status")]
    public async Task<IActionResult> Status(Guid id, UpdateOrderStatusRequest request, CancellationToken ct)
    {
        await _store.UpdateOrderStatusAsync(id, request.Status, ct);
        return NoContent();
    }
}
