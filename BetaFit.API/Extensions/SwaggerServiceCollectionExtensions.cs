using Microsoft.OpenApi.Models;

namespace BetaFit.API.Extensions;

public static class SwaggerServiceCollectionExtensions
{
    public static IServiceCollection AddBetaFitSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Beta Fit API",
                Version = "v1",
                Description = "API institucional da Beta Fit. Consumida pelo Website público e pelo Desktop administrativo. " +
                               "Projeto de portfólio: não há pagamento, checkout ou pedidos reais."
            });
        });

        return services;
    }
}
