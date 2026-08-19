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
}

public class BetaFitApiClient : IBetaFitApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContext;

    public BetaFitApiClient(HttpClient httpClient, IHttpContextAccessor httpContext)
    {
        _httpClient = httpClient;
        _httpContext = httpContext;
    }

    private void AddToken()
    {
        var token = _httpContext.HttpContext?.User.FindFirst("access_token")?.Value;
        _httpClient.DefaultRequestHeaders.Authorization =
            string.IsNullOrWhiteSpace(token) ? null : new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<List<CategoryResponse>>("api/v1/categories?onlyActive=true", cancellationToken);
        return result ?? new();
    }

    public async Task<IReadOnlyList<ProductListItemResponse>> GetFeaturedProductsAsync(int take = 8, CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<List<ProductListItemResponse>>($"api/v1/products/featured?take={take}", cancellationToken);
        return result ?? new();
    }

    public async Task<PagedResponse<ProductListItemResponse>> SearchProductsAsync(CatalogQuery query, CancellationToken cancellationToken = default)
    {
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["isActive"] = "true";
        queryString["page"] = query.Page.ToString();
        if (!string.IsNullOrWhiteSpace(query.SearchTerm)) queryString["searchTerm"] = query.SearchTerm;
        if (query.CategoryId.HasValue) queryString["categoryId"] = query.CategoryId.Value.ToString();
        if (query.Gender.HasValue) queryString["gender"] = query.Gender.Value.ToString();
        if (!string.IsNullOrWhiteSpace(query.SortBy)) queryString["sortBy"] = query.SortBy;

        var result = await _httpClient.GetFromJsonAsync<PagedResponse<ProductListItemResponse>>(
            $"api/v1/products?{queryString}", cancellationToken);
        return result ?? new();
    }

    public async Task<ProductResponse?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/v1/products/{id}", cancellationToken);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<ProductResponse>(cancellationToken: cancellationToken)
            : null;
    }

    public async Task<IReadOnlyList<ProductListItemResponse>> GetRelatedProductsAsync(Guid id, int take = 4, CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<List<ProductListItemResponse>>($"api/v1/products/{id}/related?take={take}", cancellationToken);
        return result ?? new();
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/auth/register", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken)
            : null;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/auth/login", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken)
            : null;
    }

    public async Task<IReadOnlyList<ProductListItemResponse>> GetAdminProductsAsync(CancellationToken cancellationToken = default)
    {
        AddToken();
        var result = await _httpClient.GetFromJsonAsync<PagedResponse<ProductListItemResponse>>(
            "api/v1/products?page=1&pageSize=100", cancellationToken);
        return result?.Items ?? Array.Empty<ProductListItemResponse>();
    }

    public async Task<ProductResponse?> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        AddToken();
        var response = await _httpClient.PostAsJsonAsync("api/v1/products", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<ProductResponse>(cancellationToken: cancellationToken)
            : null;
    }
}