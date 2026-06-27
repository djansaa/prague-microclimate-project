using Microsoft.Extensions.Caching.Hybrid;
using PragueMicroclimateProject.WebServices.Golemio.Clients;
using PragueMicroclimateProject.WebServices.Golemio.Models;

namespace PragueMicroclimateProject.WebServices.Golemio;

/// <summary>
/// Golemio service
/// </summary>
public partial class GolemioService
{
    private readonly ILogger<GolemioService> _logger;
    private readonly GolemioClient _client;
    private readonly HybridCache _hybridCache;

    /// <summary>
    /// ctor
    /// </summary>
    public GolemioService(GolemioClient client, HybridCache hybridCache, ILogger<GolemioService> logger)
    {
        _client = client;
        _hybridCache = hybridCache;
        _logger = logger;
    }

    /// <summary>
    /// Get all locations from Golemio.
    /// </summary>
    public async ValueTask<List<Location>> GetAllLocations(CancellationToken cancellationToken = default)
    {
        var cacheKey = "golemio:microclimate:locations:all";

        var locations = await _hybridCache.GetOrCreateAsync(
            cacheKey,
            token =>
            {
                _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);
                return new ValueTask<List<Location>?>(_client.GetMicroclimateLocationsAsync(cancellationToken: token));
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(1),
                LocalCacheExpiration = TimeSpan.FromMinutes(10)
            },
            cancellationToken: cancellationToken);

        return locations ?? [];
    }

    /// <summary>
    /// Get all locations and their points from Golemio.
    /// </summary>
    public async ValueTask<List<Point3>> GetAllLocationAndPoints(CancellationToken cancellationToken = default)
    {
        var cacheKey = "golemio:microclimate:locations-points:all";

        var points = await _hybridCache.GetOrCreateAsync(
            cacheKey,
            token =>
            {
                _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);
                return new ValueTask<List<Point3>?>(_client.GetMicroclimatePointsAsync(cancellationToken: token));
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(1),
                LocalCacheExpiration = TimeSpan.FromMinutes(10)
            },
            cancellationToken: cancellationToken);

        return points ?? [];
    }

    /// <summary>
    /// Get point measurements from Golemio.
    /// </summary>
    public async Task<List<Measurement>> GetPointMeasurements(int? locationId = null, int? pointId = null, string? measure = null, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default)
    {
        var (rangeStart, rangeEnd) = GetValidatedRange(from, to);
        var monthsToGet = GetMonthsBetween(rangeStart, rangeEnd);
        var allMeasurements = new List<Measurement>();

        foreach (var month in monthsToGet)
        {
            var monthlyMeasurements = await GetMonthlyMeasurementsAsync(locationId, pointId, measure, month, rangeStart.Offset, cancellationToken);
            allMeasurements.AddRange(monthlyMeasurements);
        }

        return FilterMeasurementsToRange(allMeasurements, rangeStart, rangeEnd);
    }

    /// <summary>
    /// Get point measurements from Golemio parallely.
    /// </summary>
    public async ValueTask<List<Measurement>> GetPointMeasurementsParallel(int? locationId = null, int? pointId = null, string? measure = null, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default)
    {
        var (rangeStart, rangeEnd) = GetValidatedRange(from, to);
        var monthsToGet = GetMonthsBetween(rangeStart, rangeEnd);
        var throttler = new SemaphoreSlim(3);

        var tasks = monthsToGet.Select(async month =>
        {
            await throttler.WaitAsync(cancellationToken);

            try
            {
                return await GetMonthlyMeasurementsAsync(locationId, pointId, measure, month, rangeStart.Offset, cancellationToken);
            }
            finally
            {
                throttler.Release();
            }
        });

        var results = await Task.WhenAll(tasks);

        return FilterMeasurementsToRange(results.Where(x => x is not null).SelectMany(x => x!), rangeStart, rangeEnd);
    }

    private async Task<List<Measurement>> GetMonthlyMeasurementsAsync(int? locationId, int? pointId, string? measure, DateOnly month, TimeSpan offset, CancellationToken cancellationToken)
    {
        var cacheKey = $"golemio:microclimate:{locationId}:{pointId}:{measure}:{month:yyyy-MM}";
        var (monthStart, monthEnd) = GetMonthBounds(month, offset);

        var measurements = await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async token =>
            {
                _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);

                var tempMeasurements = await _client.GetMicroclimateMeasurementsAsync(locationId, pointId, measure, monthStart, monthEnd, cancellationToken: token) ?? [];

                return AggregateMeasurementsByHour(tempMeasurements);
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(1),
                LocalCacheExpiration = TimeSpan.FromMinutes(10)
            },
            cancellationToken: cancellationToken);

        return measurements ?? [];
    }

    private static List<Measurement> AggregateMeasurementsByHour(IEnumerable<Measurement> measurements)
    {
        return measurements
            .Where(m => m.MeasuredAt.HasValue)
            .GroupBy(m => new DateTimeOffset(m.MeasuredAt!.Value.Year, m.MeasuredAt.Value.Month, m.MeasuredAt.Value.Day, m.MeasuredAt.Value.Hour, 0, 0, m.MeasuredAt.Value.Offset))
            .Select(group =>
            {
                var firstMeasurement = group.First();
                var values = group.Where(m => m.Value.HasValue).Select(m => m.Value!.Value).ToList();

                return new Measurement
                {
                    LocationId = firstMeasurement.LocationId,
                    PointId = firstMeasurement.PointId,
                    MeasuredAt = group.Key,
                    Measure = firstMeasurement.Measure,
                    Value = values.Count > 0 ? Math.Round(values.Average(), 2) : null,
                    Unit = firstMeasurement.Unit
                };
            })
            .OrderBy(m => m.MeasuredAt)
            .ToList();
    }

    private static List<Measurement> FilterMeasurementsToRange(IEnumerable<Measurement> measurements, DateTimeOffset from, DateTimeOffset to)
    {
        return measurements
            .Where(m => m.MeasuredAt.HasValue && m.MeasuredAt.Value >= from && m.MeasuredAt.Value <= to)
            .OrderBy(m => m.MeasuredAt)
            .ToList();
    }

    private static (DateTimeOffset Start, DateTimeOffset End) GetMonthBounds(DateOnly month, TimeSpan offset)
    {
        var monthStart = new DateTimeOffset(month.Year, month.Month, 1, 0, 0, 0, offset);
        var monthEnd = monthStart.AddMonths(1).AddTicks(-1);
        return (monthStart, monthEnd);
    }

    private static List<DateOnly> GetMonthsBetween(DateTimeOffset from, DateTimeOffset to)
    {
        var months = new List<DateOnly>();
        var current = new DateOnly(from.Year, from.Month, 1);
        var end = new DateOnly(to.Year, to.Month, 1);

        while (current <= end)
        {
            months.Add(current);
            current = current.AddMonths(1);
        }

        return months;
    }

    private static (DateTimeOffset Start, DateTimeOffset End) GetValidatedRange(DateTimeOffset? from, DateTimeOffset? to)
    {
        if (!from.HasValue || !to.HasValue)
        {
            throw new ArgumentException("'from' and 'to' must be provided.");
        }

        if (from > to)
        {
            throw new ArgumentException("'from' must be less than or equal to 'to'.");
        }

        return (from.Value, to.Value);
    }
}
