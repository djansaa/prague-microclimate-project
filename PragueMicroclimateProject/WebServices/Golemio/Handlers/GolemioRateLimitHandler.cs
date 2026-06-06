using Microsoft.Extensions.Options;
using PragueMicroclimateProject.WebServices.Golemio.Options;
using System.Net;
using System.Threading.RateLimiting;

namespace PragueMicroclimateProject.WebServices.Golemio.Handlers;

internal class GolemioRateLimitHandler : DelegatingHandler, IAsyncDisposable
{
    private readonly RateLimiter _limiter;

    public GolemioRateLimitHandler(IOptions<GolemioOptions> options)
    {
        var rateLimitOptions = options.Value.RateLimit;

        _limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            PermitLimit = rateLimitOptions.PermitLimit,
            Window = TimeSpan.FromSeconds(rateLimitOptions.WindowSeconds),
            QueueLimit = rateLimitOptions.QueueLimit,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            AutoReplenishment = true
        });
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using RateLimitLease lease = await _limiter.AcquireAsync(permitCount: 1, cancellationToken);

        if (lease.IsAcquired)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            RequestMessage = request,
            ReasonPhrase = "Client-side rate limit exceeded"
        };
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _limiter.DisposeAsync().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _limiter.Dispose();
        }
    }
}
