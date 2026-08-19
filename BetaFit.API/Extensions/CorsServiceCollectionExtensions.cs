using Microsoft.Extensions.Configuration;

namespace BetaFit.API.Extensions;

/// <summary>
/// Configura CORS liberando apenas as origens conhecidas do Website (BetaFit.UI).
/// O Desktop não sofre restrição de CORS por não ser um navegador.
/// </summary>
public static class CorsServiceCollectionExtensions
{
    public const string PolicyName = "BetaFitCorsPolicy";

    public static IServiceCollection AddBetaFitCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
