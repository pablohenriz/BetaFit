using CommunityToolkit.Mvvm.ComponentModel;

namespace BetaFit.Desktop.ViewModels;

/// <summary>
/// Base para todas as ViewModels do Desktop. Centraliza estado comum de
/// loading e mensagens de erro/sucesso exibidas na interface administrativa.
/// </summary>
public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _successMessage;

    protected void ClearMessages()
    {
        ErrorMessage = null;
        SuccessMessage = null;
    }
}
