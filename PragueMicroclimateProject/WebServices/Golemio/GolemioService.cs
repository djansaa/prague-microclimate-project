using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using PragueMicroclimateProject.Models;
using PragueMicroclimateProject.Options;
using PragueMicroclimateProject.WebServices.Golemio.Clients;
using PragueMicroclimateProject.WebServices.Golemio.Mappers;

namespace PragueMicroclimateProject.WebServices.Golemio;

/// <summary>
/// Golemio service
/// </summary>
public partial class GolemioService
{
    private readonly ILogger<GolemioService> _logger;
    private readonly GolemioClient _client;
    private readonly HybridCache _hybridCache;
    private readonly ApplicationOptions _applicationOptions;

    /// <summary>
    /// ctor
    /// </summary>
    public GolemioService(GolemioClient client, HybridCache hybridCache, ILogger<GolemioService> logger, IOptions<ApplicationOptions> applicationOptions)
    {
        _client = client;
        _hybridCache = hybridCache;
        _logger = logger;
        _applicationOptions = applicationOptions.Value;
    }

    /// <summary>
    /// Get all locations and their points from Golemio.
    /// </summary>
    public async Task<List<Location>> GetAllLocationAndPoints(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "golemio:microclimate:locations-points:all";

        var locations = await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async token =>
            {
                _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);
                var points = await _client.GetMicroclimatePointsAsync(cancellationToken: token) ?? [];
                return GolemioMicroclimateMapper.MapLocations(points);
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
    /// Get point measurements from Golemio.
    /// </summary>
    public async Task<List<Measurement>> GetPointMeasurements(int? locationId = null, int? pointId = null, string? measure = null, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default)
    {
        var (rangeStart, rangeEnd, normalizedMeasure) = ValidateMeasurementRequest(measure, from, to);
        var monthsToGet = GetMonthsBetween(rangeStart, rangeEnd);
        var allMeasurements = new List<Measurement>();

        foreach (var month in monthsToGet)
        {
            var monthlyMeasurements = await GetMonthlyMeasurementsAsync(locationId, pointId, normalizedMeasure, month, rangeStart.Offset, cancellationToken);
            allMeasurements.AddRange(monthlyMeasurements);
        }

        return FilterMeasurementsToRange(allMeasurements, rangeStart, rangeEnd);
    }

    /// <summary>
    /// Get point measurements from Golemio parallely. TEST SOLUTION.
    /// </summary>
    public async Task<List<Measurement>> GetPointMeasurementsParallel(int? locationId = null, int? pointId = null, string? measure = null, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default)
    {
        var (rangeStart, rangeEnd, normalizedMeasure) = ValidateMeasurementRequest(measure, from, to);
        var monthsToGet = GetMonthsBetween(rangeStart, rangeEnd);
        var throttler = new SemaphoreSlim(3);

        var tasks = monthsToGet.Select(async month =>
        {
            await throttler.WaitAsync(cancellationToken);

            try
            {
                return await GetMonthlyMeasurementsAsync(locationId, pointId, normalizedMeasure, month, rangeStart.Offset, cancellationToken);
            }
            finally
            {
                throttler.Release();
            }
        });

        var results = await Task.WhenAll(tasks);

        var filtered = FilterMeasurementsToRange(results.Where(x => x is not null).SelectMany(x => x!), rangeStart, rangeEnd);

        return filtered;
    }

    /// <summary>
    /// Get monthly measurements from Golemio.
    /// </summary>
    private async Task<List<Measurement>> GetMonthlyMeasurementsAsync(int? locationId, int? pointId, string? measure, DateOnly month, TimeSpan offset, CancellationToken cancellationToken)
    {
        var cacheKey = $"golemio:microclimate:{locationId}:{pointId}:{measure}:{month:yyyy-MM}";
        var (monthStart, monthEnd) = GetMonthBounds(month, offset);
        var externalMeasure = string.IsNullOrWhiteSpace(measure) ? null : MeasureTypeMapper.ToExternal(measure);

        var measurements = await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async token =>
            {
                _logger.LogDebug("Cache miss for {CacheKey}", cacheKey);

                var rawMeasurements = await _client.GetMicroclimateMeasurementsAsync(locationId, pointId, externalMeasure, monthStart, monthEnd, cancellationToken: token) ?? [];

                // map measurements to internal model
                var mappedMeasurements = GolemioMicroclimateMapper.MapMeasurements(rawMeasurements);

                return AggregateMeasurementsByHour(mappedMeasurements);
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(1),
                LocalCacheExpiration = TimeSpan.FromMinutes(10)
            },
            cancellationToken: cancellationToken);

        return measurements ?? [];
    }

    /// <summary>
    /// Get aggregated measurements by hour.
    /// </summary>
    private static List<Measurement> AggregateMeasurementsByHour(IEnumerable<Measurement> measurements)
    {
        return measurements
            .Where(measurement => measurement.Timestamp.HasValue)
            .GroupBy(measurement => new
            {
                measurement.LocationId,
                measurement.PointId,
                measurement.Type,
                measurement.Unit,
                Timestamp = new DateTimeOffset(
                    measurement.Timestamp!.Value.Year,
                    measurement.Timestamp.Value.Month,
                    measurement.Timestamp.Value.Day,
                    measurement.Timestamp.Value.Hour,
                    0,
                    0,
                    measurement.Timestamp.Value.Offset)
            })
            .Select(group =>
            {
                var values = group.Where(measurement => measurement.Value.HasValue).Select(measurement => measurement.Value!.Value).ToList();

                return new Measurement
                {
                    LocationId = group.Key.LocationId,
                    PointId = group.Key.PointId,
                    Type = group.Key.Type,
                    Unit = group.Key.Unit,
                    Timestamp = group.Key.Timestamp,
                    Value = values.Count > 0 ? Math.Round(values.Average(), 2) : null
                };
            })
            .OrderBy(measurement => measurement.Timestamp)
            .ToList();
    }

    /// <summary>
    /// Filter measurements to the specified range.
    /// </summary>
    private static List<Measurement> FilterMeasurementsToRange(IEnumerable<Measurement> measurements, DateTimeOffset from, DateTimeOffset to)
    {
        return measurements
            .Where(measurement => measurement.Timestamp.HasValue && measurement.Timestamp.Value >= from && measurement.Timestamp.Value <= to)
            .OrderBy(measurement => measurement.Timestamp)
            .ToList();
    }

    /// <summary>
    /// Get month bounds for the specified month.
    /// </summary>
    private static (DateTimeOffset Start, DateTimeOffset End) GetMonthBounds(DateOnly month, TimeSpan offset)
    {
        var monthStart = new DateTimeOffset(month.Year, month.Month, 1, 0, 0, 0, offset);
        var monthEnd = monthStart.AddMonths(1).AddTicks(-1);
        return (monthStart, monthEnd);
    }

    /// <summary>
    /// Get months between the specified datetimes.
    /// </summary>
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

    /// <summary>
    /// Validate measurement request.
    /// </summary>
    private (DateTimeOffset Start, DateTimeOffset End, string? Measure) ValidateMeasurementRequest(string? measure, DateTimeOffset? from, DateTimeOffset? to)
    {
        if (!from.HasValue)
        {
            throw new ArgumentException("'from' must be provided.", nameof(from));
        }

        if (!to.HasValue)
        {
            throw new ArgumentException("'to' must be provided.", nameof(to));
        }

        if (from > to)
        {
            throw new ArgumentException("'from' must be less than or equal to 'to'.");
        }

        var monthsToGet = GetMonthsBetween(from.Value, to.Value);
        if (monthsToGet.Count > _applicationOptions.MaxCalendarMonthsPerRequest)
        {
            throw new ArgumentException($"The selected date range can span at most {_applicationOptions.MaxCalendarMonthsPerRequest} calendar months.");
        }

        var normalizedMeasure = string.IsNullOrWhiteSpace(measure) ? null : measure.Trim();
        if (normalizedMeasure is not null && !_applicationOptions.EnabledMeasurementTypes.Contains(normalizedMeasure, StringComparer.Ordinal))
        {
            throw new ArgumentException($"'measure' must be one of the enabled measurement types: {string.Join(", ", _applicationOptions.EnabledMeasurementTypes)}.", nameof(measure));
        }

        return (from.Value, to.Value, normalizedMeasure);
    }
}
