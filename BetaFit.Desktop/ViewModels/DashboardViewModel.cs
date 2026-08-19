using BetaFit.Desktop.Models;
using BetaFit.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http;

namespace BetaFit.Desktop.ViewModels;

/// <summary>
/// ViewModel da tela inicial administrativa: mostra métricas simples
/// obtidas via GET /api/v1/dashboard/summary.
/// </summary>
public partial class DashboardViewModel : ViewModelBase
{
    private readonly IBetaFitApiService _apiService;

    [ObservableProperty]
    private DashboardSummaryResponse _summary = new();

    public DashboardViewModel(IBetaFitApiService apiService)
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
            Summary = await _apiService.GetDashboardSummaryAsync();
        }
        catch (BetaFitApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Não foi possível conectar à API da Beta Fit. Verifique se ela está em execução.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
