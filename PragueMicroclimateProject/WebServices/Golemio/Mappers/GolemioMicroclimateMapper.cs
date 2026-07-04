using PragueMicroclimateProject.Models;
using GolemioMeasurement = PragueMicroclimateProject.WebServices.Golemio.Models.Measurement;
using GolemioPoint = PragueMicroclimateProject.WebServices.Golemio.Models.Point3;

namespace PragueMicroclimateProject.WebServices.Golemio.Mappers;

/// <summary>
/// Maps Golemio microclimate models to internal application models.
/// </summary>
public static class GolemioMicroclimateMapper
{
    /// <summary>
    /// Converts a collection of Golemio points into grouped locations with points.
    /// </summary>
    public static List<Location> MapLocations(IEnumerable<GolemioPoint> points)
    {
        return points
            .GroupBy(point => point.LocationId)
            .Select(group =>
            {
                var firstPoint = group.First();

                return new Location
                {
                    Id = firstPoint.LocationId,
                    Name = firstPoint.LocationName,
                    Description = firstPoint.LocDescription,
                    Surface = firstPoint.LocSurface,
                    Points = group
                        .Select(MapPoint)
                        .OrderBy(point => point.Id)
                        .ToList()
                };
            })
            .OrderBy(location => location.Id)
            .ToList();
    }

    /// <summary>
    /// Converts Golemio measurements into internal measurement models.
    /// </summary>
    public static List<Measurement> MapMeasurements(IEnumerable<GolemioMeasurement> measurements)
    {
        return measurements
            .Select(measurement => new Measurement
            {
                LocationId = (int?)measurement.LocationId,
                PointId = (int?)measurement.PointId,
                Timestamp = measurement.MeasuredAt,
                Type = MeasureTypeMapper.ToInternal(measurement.Measure),
                Unit = MeasureUnitMapper.ToInternal(measurement.Unit),
                Value = measurement.Value
            })
            .ToList();
    }

    private static Point MapPoint(GolemioPoint point)
    {
        return new Point
        {
            Id = point.PointId,
            Name = point.PointNamed,
            Description = point.SensorPositionDetail ?? point.SensorPosition,
            SensorPosition = point.SensorPosition,
            Latitude = point.Lat,
            Longitude = point.Lng,
            MeasurementTypes = point.Measures?
                .Where(measure => !string.IsNullOrWhiteSpace(measure.Measure))
                .Select(measure => MeasureTypeMapper.ToInternal(measure.Measure!))
                .Distinct(StringComparer.Ordinal)
                .ToList() ?? []
        };
    }
}
