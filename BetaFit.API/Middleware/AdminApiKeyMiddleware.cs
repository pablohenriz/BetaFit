namespace BetaFit.API.Middleware;

public sealed class AdminApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public AdminApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsGet(context.Request.Method) ||
            context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        var configured = _configuration["Admin:ApiKey"];
        if (string.IsNullOrWhiteSpace(configured))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new { title = "Admin API key não configurada." });
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Admin-Key", out var supplied) ||
            !string.Equals(supplied.ToString(), configured, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { title = "Chave administrativa inválida." });
            return;
        }

        await _next(context);
    }
}

public static class AdminApiKeyMiddlewareExtensions
{
    public static IApplicationBuilder UseAdminApiKey(this IApplicationBuilder app)
        => app.UseMiddleware<AdminApiKeyMiddleware>();
}
