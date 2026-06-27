using PragueMicroclimateProject.WebServices.Golemio.DependencyInjection;

namespace PragueMicroclimateProject.DependencyInjection;

/// <summary>
/// Provides dependency injection setup.
/// </summary>
public static class ConfigureServiceCollection
{
    /// <summary>
    /// Register external API services in DI container.
    /// </summary>
    public static void AddWebServices(this IServiceCollection services)
    {
        services.AddGolemioService();
    }
}
