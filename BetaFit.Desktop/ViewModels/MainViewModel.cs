using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BetaFit.Desktop.ViewModels;

public enum DesktopSection
{
    Dashboard,
    Products,
    Categories
}

/// <summary>
/// ViewModel raiz da janela principal: controla qual seção (Dashboard/Produtos/Categorias)
/// está visível, funcionando como um shell simples de navegação por sidebar.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private DesktopSection _currentSection = DesktopSection.Dashboard;

    public DashboardViewModel DashboardViewModel { get; }
    public ProductListViewModel ProductListViewModel { get; }
    public CategoryListViewModel CategoryListViewModel { get; }

    public MainViewModel(
        DashboardViewModel dashboardViewModel,
        ProductListViewModel productListViewModel,
        CategoryListViewModel categoryListViewModel)
    {
        DashboardViewModel = dashboardViewModel;
        ProductListViewModel = productListViewModel;
        CategoryListViewModel = categoryListViewModel;
    }

    [RelayCommand]
    private async Task NavigateAsync(string section)
    {
        CurrentSection = Enum.Parse<DesktopSection>(section);

        switch (CurrentSection)
        {
            case DesktopSection.Dashboard:
                await DashboardViewModel.LoadCommand.ExecuteAsync(null);
                break;
            case DesktopSection.Products:
                await ProductListViewModel.LoadCommand.ExecuteAsync(null);
                break;
            case DesktopSection.Categories:
                await CategoryListViewModel.LoadCommand.ExecuteAsync(null);
                break;
        }
    }
}
