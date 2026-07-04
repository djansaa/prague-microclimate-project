namespace PragueMicroclimateProject.WebServices.Golemio.Options;

/// <summary>
/// Golemio API options
/// </summary>
public class GolemioOptions
{
    /// <summary>
    /// Section name in configuration
    /// </summary>
    public const string SectionName = "GolemioService";

    /// <summary>
    /// Base URL
    /// </summary>
    public string BaseUrl { get; set; }

    /// <summary>
    /// API key
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Rate limit options
    /// </summary>
    public RateLimitOptions RateLimit { get; set; }
}
