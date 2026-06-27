using Microsoft.AspNetCore.WebUtilities;
using PragueMicroclimateProject.WebServices.Golemio.Models;
using System.Globalization;

namespace PragueMicroclimateProject.WebServices.Golemio.Clients;

/// <summary>
/// Golemio API client
/// </summary>
public class GolemioClient
{
    private readonly HttpClient _client;

    /// <summary>
    /// ctor
    /// </summary>
    public GolemioClient(HttpClient client)
    {
        _client = client;
    }

    #region Microclimate (v2)

    /// <summary>
    /// GET All Microclimate Sensor Locations. Optionally filter by locationId otherwise returns all locations.
    /// </summary>
    public Task<List<Location>?> GetMicroclimateLocationsAsync(int? locationId = null, CancellationToken cancellationToken = default)
    {
        var endpoint = "/v2/microclimate/locations";

        if (locationId.HasValue)
        {
            endpoint = QueryHelpers.AddQueryString(endpoint, "locationId", locationId.Value.ToString(CultureInfo.InvariantCulture));
        }

        return _client.GetFromJsonAsync<List<Location>>(endpoint, cancellationToken);
    }

    /// <summary>
    /// GET Microclimate Sensor Points. Optionally filter by locationId and /or pointId.
    /// Return all information about location and its point except measurements.
    /// </summary>
    public Task<List<Point3>?> GetMicroclimatePointsAsync(int? locationId = null, int? pointId = null, CancellationToken cancellationToken = default)
    {
        // Warning: Golemio API has wrong docs description - it returns list of points, not single point.
        var endpoint = "/v2/microclimate/points";

        if (locationId.HasValue)
        {
            endpoint = QueryHelpers.AddQueryString(endpoint, "locationId", locationId.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (pointId.HasValue)
        {
            endpoint = QueryHelpers.AddQueryString(endpoint, "pointId", pointId.Value.ToString(CultureInfo.InvariantCulture));
        }

        return _client.GetFromJsonAsync<List<Point3>>(endpoint, cancellationToken);
    }

    /// <summary>
    /// GET All Microclimate Sensor Measurements. Optionally filter by locationId, pointId, measure, from and to.
    /// </summary>
    public Task<List<Measurement>?> GetMicroclimateMeasurementsAsync(int? locationId = null, int? pointId = null, string? measure = null, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default)
    {
        var endpoint = "/v2/microclimate/measurements";

        if (locationId.HasValue)
        {
            endpoint = QueryHelpers.AddQueryString(endpoint, "locationId", locationId.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (pointId.HasValue)
        {
            endpoint = QueryHelpers.AddQueryString(endpoint, "pointId", pointId.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (!string.IsNullOrWhiteSpace(measure))
        {
            endpoint = QueryHelpers.AddQueryString(endpoint, "measure", measure);
        }

        if (from.HasValue)
        {
            endpoint = QueryHelpers.AddQueryString(endpoint, "from", from.Value.ToString("O", CultureInfo.InvariantCulture));
        }

        if (to.HasValue)
        {
            endpoint = QueryHelpers.AddQueryString(endpoint, "to", to.Value.ToString("O", CultureInfo.InvariantCulture));
        }

        return _client.GetFromJsonAsync<List<Measurement>>(endpoint, cancellationToken);
    }

    #endregion
}
