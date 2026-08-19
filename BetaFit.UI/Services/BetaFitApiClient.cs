using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;
using BetaFit.UI.Models;
using Microsoft.AspNetCore.Http;

namespace BetaFit.UI.Services;

public interface IBetaFitApiClient
{
    Task<IReadOnlyList<CategoryResponse>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductListItemResponse>> GetFeaturedProductsAsync(int take = 8, CancellationToken cancellationToken = default);
    Task<PagedResponse<ProductListItemResponse>> SearchProductsAsync(CatalogQuery query, CancellationToken cancellationToken = default);
    Task<ProductResponse?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductListItemResponse>> GetRelatedProductsAsync(Guid id, int take = 4, CancellationToken cancellationToken = default);
    Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductListItemResponse>> GetAdminProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductResponse?> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<ProfileResponse?> GetProfileAsync(CancellationToken ct = default);
    Task<CartApiResponse?> GetCartAsync(CancellationToken ct = default);
    Task<CartApiResponse?> AddCartItemAsync(Guid productId, int quantity = 1, CancellationToken ct = default);
    Task<CartApiResponse?> UpdateCartItemAsync(Guid itemId, int quantity, CancellationToken ct = default);
    Task<CartApiResponse?> RemoveCartItemAsync(Guid itemId, CancellationToken ct = default);
    Task ClearCartAsync(CancellationToken ct = default);
    Task<OrderDetail?> CreateOrderAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OrderListItem>> GetMyOrdersAsync(CancellationToken ct = default);
    Task<OrderDetail?> GetMyOrderAsync(Guid id, CancellationToken ct = default);
    Task<AdminDashboard?> GetAdminDashboardAsync(CancellationToken ct = default);
    Task<IReadOnlyList<AdminUser>> GetAdminUsersAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OrderListItem>> GetAdminOrdersAsync(CancellationToken ct = default);
    Task<OrderDetail?> GetAdminOrderAsync(Guid id, CancellationToken ct = default);
    Task<bool> UpdateOrderStatusAsync(Guid id, string status, CancellationToken ct = default);
    Task<bool> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    Task<bool> DeleteProductAsync(Guid id, CancellationToken ct = default);
}

public class BetaFitApiClient : IBetaFitApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContext;
    public BetaFitApiClient(HttpClient httpClient, IHttpContextAccessor httpContext) { _httpClient = httpClient; _httpContext = httpContext; }

    private void AddToken()
    {
        var token = _httpContext.HttpContext?.User.FindFirst("access_token")?.Value;
        _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token) ? null : new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetActiveCategoriesAsync(CancellationToken ct = default) => await _httpClient.GetFromJsonAsync<List<CategoryResponse>>("api/v1/categories?onlyActive=true", ct) ?? new();
    public async Task<IReadOnlyList<ProductListItemResponse>> GetFeaturedProductsAsync(int take = 8, CancellationToken ct = default) => await _httpClient.GetFromJsonAsync<List<ProductListItemResponse>>($"api/v1/products/featured?take={take}", ct) ?? new();
    public async Task<PagedResponse<ProductListItemResponse>> SearchProductsAsync(CatalogQuery query, CancellationToken ct = default)
    {
        var q = HttpUtility.ParseQueryString(string.Empty); q["isActive"] = "true"; q["page"] = query.Page.ToString();
        if (!string.IsNullOrWhiteSpace(query.SearchTerm)) q["searchTerm"] = query.SearchTerm;
        if (query.CategoryId.HasValue) q["categoryId"] = query.CategoryId.Value.ToString();
        if (query.Gender.HasValue) q["gender"] = query.Gender.Value.ToString();
        if (!string.IsNullOrWhiteSpace(query.SortBy)) q["sortBy"] = query.SortBy;
        return await _httpClient.GetFromJsonAsync<PagedResponse<ProductListItemResponse>>($"api/v1/products?{q}", ct) ?? new();
    }
    public async Task<ProductResponse?> GetProductByIdAsync(Guid id, CancellationToken ct = default) { var r = await _httpClient.GetAsync($"api/v1/products/{id}", ct); return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<ProductResponse>(cancellationToken: ct) : null; }
    public async Task<IReadOnlyList<ProductListItemResponse>> GetRelatedProductsAsync(Guid id, int take = 4, CancellationToken ct = default) => await _httpClient.GetFromJsonAsync<List<ProductListItemResponse>>($"api/v1/products/{id}/related?take={take}", ct) ?? new();
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken ct = default) { var r = await _httpClient.PostAsJsonAsync("api/v1/auth/register", request, ct); return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct) : null; }
    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default) { var r = await _httpClient.PostAsJsonAsync("api/v1/auth/login", request, ct); return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct) : null; }
    public async Task<IReadOnlyList<ProductListItemResponse>> GetAdminProductsAsync(CancellationToken ct = default) { AddToken(); var r = await _httpClient.GetFromJsonAsync<PagedResponse<ProductListItemResponse>>("api/v1/products?page=1&pageSize=100", ct); return r?.Items ?? Array.Empty<ProductListItemResponse>(); }
    public async Task<ProductResponse?> CreateProductAsync(CreateProductRequest request, CancellationToken ct = default) { AddToken(); var r = await _httpClient.PostAsJsonAsync("api/v1/products", request, ct); return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<ProductResponse>(cancellationToken: ct) : null; }

    private async Task<T?> GetAuth<T>(string url, CancellationToken ct) { AddToken(); var r = await _httpClient.GetAsync(url, ct); return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<T>(cancellationToken: ct) : default; }
    public Task<ProfileResponse?> GetProfileAsync(CancellationToken ct = default) => GetAuth<ProfileResponse>("api/v1/store/profile", ct);
    public Task<CartApiResponse?> GetCartAsync(CancellationToken ct = default) => GetAuth<CartApiResponse>("api/v1/store/cart", ct);
    public async Task<CartApiResponse?> AddCartItemAsync(Guid productId, int quantity = 1, CancellationToken ct = default) { AddToken(); var r = await _httpClient.PostAsJsonAsync("api/v1/store/cart/items", new AddCartItemRequest { ProductId = productId, Quantity = quantity }, ct); return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<CartApiResponse>(cancellationToken: ct) : null; }
    public async Task<CartApiResponse?> UpdateCartItemAsync(Guid itemId, int quantity, CancellationToken ct = default) { AddToken(); var r = await _httpClient.PutAsJsonAsync($"api/v1/store/cart/items/{itemId}", new UpdateCartItemRequest { Quantity = quantity }, ct); return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<CartApiResponse>(cancellationToken: ct) : null; }
    public async Task<CartApiResponse?> RemoveCartItemAsync(Guid itemId, CancellationToken ct = default) { AddToken(); var r = await _httpClient.DeleteAsync($"api/v1/store/cart/items/{itemId}", ct); return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<CartApiResponse>(cancellationToken: ct) : null; }
    public async Task ClearCartAsync(CancellationToken ct = default) { AddToken(); await _httpClient.DeleteAsync("api/v1/store/cart", ct); }
    public async Task<OrderDetail?> CreateOrderAsync(CancellationToken ct = default) { AddToken(); var r = await _httpClient.PostAsync("api/v1/store/orders", null, ct); return r.IsSuccessStatusCode ? await r.Content.ReadFromJsonAsync<OrderDetail>(cancellationToken: ct) : null; }
    public async Task<IReadOnlyList<OrderListItem>> GetMyOrdersAsync(CancellationToken ct = default) => await GetAuth<List<OrderListItem>>("api/v1/store/orders", ct) ?? new();
    public Task<OrderDetail?> GetMyOrderAsync(Guid id, CancellationToken ct = default) => GetAuth<OrderDetail>($"api/v1/store/orders/{id}", ct);
    public Task<AdminDashboard?> GetAdminDashboardAsync(CancellationToken ct = default) => GetAuth<AdminDashboard>("api/v1/admin/dashboard", ct);
    public async Task<IReadOnlyList<AdminUser>> GetAdminUsersAsync(CancellationToken ct = default) => await GetAuth<List<AdminUser>>("api/v1/admin/users", ct) ?? new();
    public async Task<IReadOnlyList<OrderListItem>> GetAdminOrdersAsync(CancellationToken ct = default) => await GetAuth<List<OrderListItem>>("api/v1/admin/orders", ct) ?? new();
    public Task<OrderDetail?> GetAdminOrderAsync(Guid id, CancellationToken ct = default) => GetAuth<OrderDetail>($"api/v1/admin/orders/{id}", ct);
    public async Task<bool> UpdateOrderStatusAsync(Guid id, string status, CancellationToken ct = default) { AddToken(); var r = await _httpClient.PatchAsJsonAsync($"api/v1/admin/orders/{id}/status", new UpdateOrderStatusRequest { Status = status }, ct); return r.IsSuccessStatusCode; }
    public async Task<bool> UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default) { AddToken(); var r = await _httpClient.PutAsJsonAsync($"api/v1/products/{id}", request, ct); return r.IsSuccessStatusCode; }
    public async Task<bool> DeleteProductAsync(Guid id, CancellationToken ct = default) { AddToken(); var r = await _httpClient.DeleteAsync($"api/v1/products/{id}", ct); return r.IsSuccessStatusCode; }
}
