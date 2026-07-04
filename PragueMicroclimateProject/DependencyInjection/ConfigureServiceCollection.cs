using PragueMicroclimateProject.Options;
using PragueMicroclimateProject.WebServices.Golemio.DependencyInjection;

namespace PragueMicroclimateProject.DependencyInjection;

/// <summary>
/// Provides dependency injection setup.
/// </summary>
public static class ConfigureServiceCollection
{
    /// <summary>
    /// Register application options in DI container.
    /// </summary>
    public static void AddApplicationOptions(this IServiceCollection services)
    {
        services.AddOptions<ApplicationOptions>().BindConfiguration(ApplicationOptions.SectionName);
    }

    /// <summary>
    /// Register external API services in DI container.
    /// </summary>
    public static void AddWebServices(this IServiceCollection services)
    {
        services.AddGolemioService();
    }
}
