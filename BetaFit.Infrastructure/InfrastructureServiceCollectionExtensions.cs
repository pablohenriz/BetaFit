using BetaFit.Domain.Interfaces;
using BetaFit.Infrastructure.Context;
using BetaFit.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BetaFit.Infrastructure;

/// <summary>
/// Ponto único de registro dos serviços da camada Infrastructure no container de DI.
/// Chamado a partir da BetaFit.API (Program.cs). Nenhuma outra camada referencia este projeto
/// diretamente além da API, o que garante que EF Core/SQL Server fiquem isolados aqui.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("A connection string 'DefaultConnection' não foi configurada.");

        services.AddDbContext<BetaFitDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(BetaFitDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BetaFitDbContext>());
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
