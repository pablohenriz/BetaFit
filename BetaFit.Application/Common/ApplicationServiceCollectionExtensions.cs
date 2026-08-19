using BetaFit.Application.Interfaces;
using BetaFit.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BetaFit.Application.Common;

/// <summary>
/// Ponto único de registro dos serviços da camada Application no container de DI.
/// Chamado a partir da BetaFit.API (Program.cs).
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddValidatorsFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly);

        return services;
    }
}
