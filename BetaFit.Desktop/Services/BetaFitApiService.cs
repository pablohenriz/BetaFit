using System.Net.Http.Json;
using BetaFit.Desktop.Models;
using System.Net.Http;

namespace BetaFit.Desktop.Services;

/// <summary>
/// Exceção lançada quando a API retorna um erro de negócio ou validação (400/404).
/// Permite que a ViewModel exiba a mensagem correta ao usuário administrador.
/// </summary>
public class BetaFitApiException : Exception
{
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }

    public BetaFitApiException(string message, IReadOnlyDictionary<string, string[]>? validationErrors = null)
        : base(message)
    {
        ValidationErrors = validationErrors;
    }
}

/// <summary>
/// Único ponto de comunicação do Desktop com o backend.
/// O Desktop NÃO acessa DbContext, SQL Server ou connection strings -
/// toda operação de Produtos e Categorias passa por aqui, via HTTP contra a BetaFit.API.
/// </summary>
public interface IBetaFitApiService
{
    Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync(bool onlyActive = false);
    Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
    Task<CategoryResponse> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request);
    Task ActivateCategoryAsync(Guid id);
    Task DeactivateCategoryAsync(Guid id);
    Task DeleteCategoryAsync(Guid id);

    Task<PagedResponse<ProductListItemResponse>> SearchProductsAsync(string? searchTerm, Guid? categoryId, int page = 1, int pageSize = 20);
    Task<ProductResponse> GetProductByIdAsync(Guid id);
    Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
    Task<ProductResponse> UpdateProductAsync(Guid id, UpdateProductRequest request);
    Task ActivateProductAsync(Guid id);
    Task DeactivateProductAsync(Guid id);
    Task SetFeaturedAsync(Guid id, bool isFeatured);
    Task DeleteProductAsync(Guid id);

    Task<DashboardSummaryResponse> GetDashboardSummaryAsync();
}

public class BetaFitApiService : IBetaFitApiService
{
    private readonly HttpClient _httpClient;

    public BetaFitApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync(bool onlyActive = false)
    {
        var response = await _httpClient.GetAsync($"api/v1/categories?onlyActive={onlyActive}");
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<List<CategoryResponse>>() ?? new List<CategoryResponse>();
    }

    public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/categories", request);
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
    }

    public async Task<CategoryResponse> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/v1/categories/{id}", request);
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
    }

    public async Task ActivateCategoryAsync(Guid id)
    {
        var response = await _httpClient.PatchAsync($"api/v1/categories/{id}/activate", null);
        await EnsureSuccessAsync(response);
    }

    public async Task DeactivateCategoryAsync(Guid id)
    {
        var response = await _httpClient.PatchAsync($"api/v1/categories/{id}/deactivate", null);
        await EnsureSuccessAsync(response);
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/v1/categories/{id}");
        await EnsureSuccessAsync(response);
    }

    public async Task<PagedResponse<ProductListItemResponse>> SearchProductsAsync(string? searchTerm, Guid? categoryId, int page = 1, int pageSize = 20)
    {
        var url = $"api/v1/products?page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(searchTerm))
            url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";

        if (categoryId.HasValue)
            url += $"&categoryId={categoryId.Value}";

        var response = await _httpClient.GetAsync(url);
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<PagedResponse<ProductListItemResponse>>())!;
    }

    public async Task<ProductResponse> GetProductByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/v1/products/{id}");
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<ProductResponse>())!;
    }

    public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/products", request);
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<ProductResponse>())!;
    }

    public async Task<ProductResponse> UpdateProductAsync(Guid id, UpdateProductRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/v1/products/{id}", request);
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<ProductResponse>())!;
    }

    public async Task ActivateProductAsync(Guid id)
    {
        var response = await _httpClient.PatchAsync($"api/v1/products/{id}/activate", null);
        await EnsureSuccessAsync(response);
    }

    public async Task DeactivateProductAsync(Guid id)
    {
        var response = await _httpClient.PatchAsync($"api/v1/products/{id}/deactivate", null);
        await EnsureSuccessAsync(response);
    }

    public async Task SetFeaturedAsync(Guid id, bool isFeatured)
    {
        var response = await _httpClient.PatchAsync($"api/v1/products/{id}/featured?isFeatured={isFeatured}", null);
        await EnsureSuccessAsync(response);
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/v1/products/{id}");
        await EnsureSuccessAsync(response);
    }

    public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync()
    {
        var response = await _httpClient.GetAsync("api/v1/dashboard/summary");
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<DashboardSummaryResponse>())!;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        throw new BetaFitApiException(
            error?.Title ?? "Ocorreu um erro ao comunicar com a API.",
            error?.Errors);
    }
}
