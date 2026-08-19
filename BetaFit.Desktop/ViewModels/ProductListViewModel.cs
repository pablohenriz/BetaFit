using System.Collections.ObjectModel;
using BetaFit.Desktop.Models;
using BetaFit.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http;

namespace BetaFit.Desktop.ViewModels;

/// <summary>
/// ViewModel da tela de Produtos: listar (com busca e paginação simples),
/// criar, editar, ativar/desativar, destacar e excluir.
/// </summary>
public partial class ProductListViewModel : ViewModelBase
{
    private readonly IBetaFitApiService _apiService;

    public ObservableCollection<ProductListItemResponse> Products { get; } = new();
    public ObservableCollection<CategoryResponse> Categories { get; } = new();

    [ObservableProperty] private string? _searchTerm;
    [ObservableProperty] private int _page = 1;
    [ObservableProperty] private int _totalPages = 1;

    [ObservableProperty] private ProductResponse? _selectedProduct;
    [ObservableProperty] private bool _isEditing;

    // Campos do formulário (criação/edição)
    [ObservableProperty] private string _formName = string.Empty;
    [ObservableProperty] private string _formDescription = string.Empty;
    [ObservableProperty] private decimal _formPrice;
    [ObservableProperty] private string? _formImageUrl;
    [ObservableProperty] private Gender _formGender;
    [ObservableProperty] private CategoryResponse? _formCategory;
    [ObservableProperty] private bool _formIsFeatured;

    public IReadOnlyList<Gender> GenderOptions { get; } = Enum.GetValues<Gender>();

    public ProductListViewModel(IBetaFitApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        ClearMessages();

        try
        {
            var categories = await _apiService.GetCategoriesAsync(onlyActive: false);
            Categories.Clear();
            foreach (var category in categories)
                Categories.Add(category);

            var result = await _apiService.SearchProductsAsync(SearchTerm, categoryId: null, page: Page, pageSize: 20);

            Products.Clear();
            foreach (var product in result.Items)
                Products.Add(product);

            TotalPages = Math.Max(result.TotalPages, 1);
        }
        catch (BetaFitApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Não foi possível conectar à API da Beta Fit.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        Page = 1;
        await LoadAsync();
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (Page < TotalPages)
        {
            Page++;
            await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (Page > 1)
        {
            Page--;
            await LoadAsync();
        }
    }

    [RelayCommand]
    private void NewProduct()
    {
        SelectedProduct = null;
        FormName = string.Empty;
        FormDescription = string.Empty;
        FormPrice = 0;
        FormImageUrl = null;
        FormGender = Gender.Unissex;
        FormCategory = Categories.FirstOrDefault();
        FormIsFeatured = false;
        IsEditing = true;
        ClearMessages();
    }

    [RelayCommand]
    private async Task EditProductAsync(ProductListItemResponse item)
    {
        ClearMessages();
        IsLoading = true;
        try
        {
            var product = await _apiService.GetProductByIdAsync(item.Id);
            SelectedProduct = product;
            FormName = product.Name;
            FormDescription = product.Description;
            FormPrice = product.Price;
            FormImageUrl = product.ImageUrl;
            FormGender = product.Gender;
            FormCategory = Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            FormIsFeatured = product.IsFeatured;
            IsEditing = true;
        }
        catch (BetaFitApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        SelectedProduct = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ClearMessages();

        if (string.IsNullOrWhiteSpace(FormName) || FormCategory is null)
        {
            ErrorMessage = "Nome e categoria são obrigatórios.";
            return;
        }

        IsLoading = true;
        try
        {
            if (SelectedProduct is null)
            {
                await _apiService.CreateProductAsync(new CreateProductRequest
                {
                    Name = FormName,
                    Description = FormDescription,
                    Price = FormPrice,
                    ImageUrl = FormImageUrl,
                    CategoryId = FormCategory.Id,
                    Gender = FormGender,
                    IsFeatured = FormIsFeatured
                });
                SuccessMessage = "Produto criado com sucesso.";
            }
            else
            {
                await _apiService.UpdateProductAsync(SelectedProduct.Id, new UpdateProductRequest
                {
                    Name = FormName,
                    Description = FormDescription,
                    Price = FormPrice,
                    ImageUrl = FormImageUrl,
                    CategoryId = FormCategory.Id,
                    Gender = FormGender
                });

                await _apiService.SetFeaturedAsync(SelectedProduct.Id, FormIsFeatured);
                SuccessMessage = "Produto atualizado com sucesso.";
            }

            IsEditing = false;
            await LoadAsync();
        }
        catch (BetaFitApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ToggleActiveAsync(ProductListItemResponse item)
    {
        ClearMessages();
        IsLoading = true;
        try
        {
            if (item.IsActive)
                await _apiService.DeactivateProductAsync(item.Id);
            else
                await _apiService.ActivateProductAsync(item.Id);

            await LoadAsync();
        }
        catch (BetaFitApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ToggleFeaturedAsync(ProductListItemResponse item)
    {
        ClearMessages();
        IsLoading = true;
        try
        {
            await _apiService.SetFeaturedAsync(item.Id, !item.IsFeatured);
            await LoadAsync();
        }
        catch (BetaFitApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(ProductListItemResponse item)
    {
        ClearMessages();
        IsLoading = true;
        try
        {
            await _apiService.DeleteProductAsync(item.Id);
            SuccessMessage = "Produto excluído com sucesso.";
            await LoadAsync();
        }
        catch (BetaFitApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
