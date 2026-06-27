using Microsoft.Extensions.Options;
using PragueMicroclimateProject.WebServices.Golemio.Clients;
using PragueMicroclimateProject.WebServices.Golemio.Handlers;
using PragueMicroclimateProject.WebServices.Golemio.Options;
using System.Threading.RateLimiting;

namespace PragueMicroclimateProject.WebServices.Golemio.DependencyInjection;

/// <summary>
/// Golemio service DI
/// </summary>
public static class ConfigureServiceCollection
{
    /// <summary>
    /// Registers Golemio service in DI container.
    /// </summary>
    public static void AddGolemioService(this IServiceCollection services)
    {
        services.AddOptions<GolemioOptions>().BindConfiguration(GolemioOptions.SectionName);

        services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<GolemioOptions>>().Value;
            var rateLimitOptions = options.RateLimit;

            return new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
            {
                PermitLimit = rateLimitOptions.PermitLimit,
                Window = TimeSpan.FromSeconds(rateLimitOptions.WindowSeconds),
                SegmentsPerWindow = Math.Max(1, rateLimitOptions.WindowSeconds),
                QueueLimit = rateLimitOptions.QueueLimit,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                AutoReplenishment = true
            });
        });

        services.AddHttpClient<GolemioClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<GolemioOptions>>().Value;

            if (!string.IsNullOrEmpty(options.BaseUrl))
            {
                client.BaseAddress = new Uri(options.BaseUrl);
            }
        })
        .AddHttpMessageHandler<GolemioRateLimitHandler>()
        .AddHttpMessageHandler<GolemioAuthenticationHandler>();

        services.AddTransient<GolemioAuthenticationHandler>();
        services.AddTransient<GolemioRateLimitHandler>();

        services.AddScoped<GolemioService>();
    }
}
