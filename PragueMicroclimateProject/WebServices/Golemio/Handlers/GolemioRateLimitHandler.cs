using System.Net;
using System.Threading.RateLimiting;

namespace PragueMicroclimateProject.WebServices.Golemio.Handlers;

internal class GolemioRateLimitHandler : DelegatingHandler
{
    private readonly SlidingWindowRateLimiter _limiter;
    private readonly ILogger<GolemioRateLimitHandler> _logger;

    public GolemioRateLimitHandler(SlidingWindowRateLimiter limiter, ILogger<GolemioRateLimitHandler> logger)
    {
        _limiter = limiter;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using RateLimitLease lease = await _limiter.AcquireAsync(permitCount: 1, cancellationToken).ConfigureAwait(false);

        if (lease.IsAcquired)
        {
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        _logger.LogDebug("Client-side Golemio rate limit queue is full for {Method} {Uri}. Rejecting request before sending it upstream.", 
            request.Method, request.RequestUri);

        return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            RequestMessage = request,
            ReasonPhrase = "Client-side rate limit exceeded"
        };
    }
}
