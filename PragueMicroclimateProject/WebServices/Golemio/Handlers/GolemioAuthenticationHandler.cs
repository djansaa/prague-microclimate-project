using Microsoft.Extensions.Options;
using PragueMicroclimateProject.WebServices.Golemio.Options;

namespace PragueMicroclimateProject.WebServices.Golemio.Handlers;

/// <summary>
/// Golemio authentication handler. Adds API key to outgoing requests.
/// </summary>
public class GolemioAuthenticationHandler : DelegatingHandler
{
    private const string ApiKeyHeaderName = "X-Access-Token";
    private readonly GolemioOptions _options;

    /// <inheritdoc/>
    public GolemioAuthenticationHandler(IOptions<GolemioOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!request.Headers.Contains(ApiKeyHeaderName))
        {
            // Get API key from client configuration
            var apiKey = _options.ApiKey;

            if (!string.IsNullOrEmpty(apiKey))
            {
                request.Headers.Add(ApiKeyHeaderName, apiKey);
            }
        }
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
