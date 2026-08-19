using System.Windows;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.ViewModels;
using BetaFit.Desktop.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BetaFit.Desktop;

/// <summary>
/// Bootstrap da aplicação Desktop. Configura o container de Dependency Injection
/// registrando o HttpClient tipado para a BetaFit.API e as ViewModels da aplicação.
/// Não há registro de DbContext, repositórios ou qualquer dependência de banco de dados.
/// </summary>
public partial class App : Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((context, services) =>
            {
                var apiBaseUrl = context.Configuration["BetaFitApi:BaseUrl"]
                    ?? throw new InvalidOperationException("Configuração 'BetaFitApi:BaseUrl' não encontrada.");

                services.AddTransient<AdminApiKeyHandler>();

                services.AddHttpClient<IBetaFitApiService, BetaFitApiService>(client =>
                {
                    client.BaseAddress = new Uri(apiBaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(15);
                }).AddHttpMessageHandler<AdminApiKeyHandler>();

                services.AddTransient<DashboardViewModel>();
                services.AddTransient<ProductListViewModel>();
                services.AddTransient<CategoryListViewModel>();
                services.AddSingleton<MainViewModel>();

                services.AddSingleton<MainWindow>();
            })
            .Build();

        await _host.StartAsync();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.DataContext = _host.Services.GetRequiredService<MainViewModel>();
        mainWindow.Show();

        var mainViewModel = (MainViewModel)mainWindow.DataContext;
        await mainViewModel.DashboardViewModel.LoadCommand.ExecuteAsync(null);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
