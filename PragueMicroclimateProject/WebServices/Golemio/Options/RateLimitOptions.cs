
namespace PragueMicroclimateProject.WebServices.Golemio.Options;

/// <summary>
/// Rate limit options for Golemio API client.
/// </summary>
public class RateLimitOptions
{
    /// <summary>
    /// Permit limit
    /// </summary>
    public int PermitLimit { get; init; }

    /// <summary>
    /// Window in seconds
    /// </summary>
    public int WindowSeconds { get; init; }

    /// <summary>
    /// Queue limit
    /// </summary>
    public int QueueLimit { get; init; }
}
