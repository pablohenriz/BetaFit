using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace BetaFit.Desktop.Services;

public sealed class AdminApiKeyHandler : DelegatingHandler
{
    private readonly IConfiguration _configuration;
    public AdminApiKeyHandler(IConfiguration configuration) => _configuration = configuration;
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var key = _configuration["BetaFitApi:AdminKey"];
        if (!string.IsNullOrWhiteSpace(key))
            request.Headers.TryAddWithoutValidation("X-Admin-Key", key);
        return base.SendAsync(request, cancellationToken);
    }
}
