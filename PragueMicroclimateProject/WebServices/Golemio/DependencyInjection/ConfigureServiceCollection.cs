using Microsoft.Extensions.Options;
using PragueMicroclimateProject.WebServices.Golemio.Clients;
using PragueMicroclimateProject.WebServices.Golemio.Handlers;
using PragueMicroclimateProject.WebServices.Golemio.Options;

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
    }
}
