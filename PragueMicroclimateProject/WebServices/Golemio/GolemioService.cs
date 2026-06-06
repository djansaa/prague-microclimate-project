using PragueMicroclimateProject.WebServices.Golemio.Clients;

namespace PragueMicroclimateProject.WebServices.Golemio;

/// <summary>
/// Golemio service
/// </summary>
public class GolemioService
{
    private readonly GolemioClient _client;

    /// <summary>
    /// ctor
    /// </summary>
    public GolemioService(GolemioClient client)
    {
        _client = client;
    }
}
