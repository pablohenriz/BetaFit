using BetaFit.Application.Interfaces;
using BetaFit.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BetaFit.Application.Common;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<StoreService>();
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly);
        return services;
    }
}
