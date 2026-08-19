using System.Collections.ObjectModel;
using BetaFit.Desktop.Models;
using BetaFit.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http;

namespace BetaFit.Desktop.ViewModels;

/// <summary>
/// ViewModel da tela de Categorias: listar, criar, editar, ativar/desativar e excluir.
/// Todas as operações são feitas via IBetaFitApiService (HTTP contra a API).
/// </summary>
public partial class CategoryListViewModel : ViewModelBase
{
    private readonly IBetaFitApiService _apiService;

    public ObservableCollection<CategoryResponse> Categories { get; } = new();

    [ObservableProperty]
    private string? _searchTerm;

    [ObservableProperty]
    private CategoryResponse? _selectedCategory;

    // Campos do formulário (criação/edição)
    [ObservableProperty] private string _formName = string.Empty;
    [ObservableProperty] private string _formDescription = string.Empty;
    [ObservableProperty] private string? _formImageUrl;
    [ObservableProperty] private bool _isEditing;

    public CategoryListViewModel(IBetaFitApiService apiService)
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
            var filtered = string.IsNullOrWhiteSpace(SearchTerm)
                ? categories
                : categories.Where(c => c.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

            Categories.Clear();
            foreach (var category in filtered)
                Categories.Add(category);
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
    private void NewCategory()
    {
        SelectedCategory = null;
        FormName = string.Empty;
        FormDescription = string.Empty;
        FormImageUrl = null;
        IsEditing = true;
        ClearMessages();
    }

    [RelayCommand]
    private void EditCategory(CategoryResponse category)
    {
        SelectedCategory = category;
        FormName = category.Name;
        FormDescription = category.Description;
        FormImageUrl = category.ImageUrl;
        IsEditing = true;
        ClearMessages();
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        SelectedCategory = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        ClearMessages();

        if (string.IsNullOrWhiteSpace(FormName))
        {
            ErrorMessage = "O nome da categoria é obrigatório.";
            return;
        }

        IsLoading = true;
        try
        {
            if (SelectedCategory is null)
            {
                await _apiService.CreateCategoryAsync(new CreateCategoryRequest
                {
                    Name = FormName,
                    Description = FormDescription,
                    ImageUrl = FormImageUrl
                });
                SuccessMessage = "Categoria criada com sucesso.";
            }
            else
            {
                await _apiService.UpdateCategoryAsync(SelectedCategory.Id, new UpdateCategoryRequest
                {
                    Name = FormName,
                    Description = FormDescription,
                    ImageUrl = FormImageUrl
                });
                SuccessMessage = "Categoria atualizada com sucesso.";
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
    private async Task ToggleActiveAsync(CategoryResponse category)
    {
        ClearMessages();
        IsLoading = true;
        try
        {
            if (category.IsActive)
                await _apiService.DeactivateCategoryAsync(category.Id);
            else
                await _apiService.ActivateCategoryAsync(category.Id);

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
    private async Task DeleteAsync(CategoryResponse category)
    {
        ClearMessages();
        IsLoading = true;
        try
        {
            await _apiService.DeleteCategoryAsync(category.Id);
            SuccessMessage = "Categoria excluída com sucesso.";
            await LoadAsync();
        }
        catch (BetaFitApiException ex)
        {
            // Ex: categoria com produtos vinculados - mensagem vem da regra de negócio no Domain.
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
