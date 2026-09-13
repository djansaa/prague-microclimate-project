using PragueMicroclimateProject.Models.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Mappers;
using GolemioMeasurement = PragueMicroclimateProject.WebServices.Golemio.Models.Measurement;
using GolemioPoint = PragueMicroclimateProject.WebServices.Golemio.Models.Point3;
using GolemioPointMeasure = PragueMicroclimateProject.WebServices.Golemio.Models.Point3Measure;

namespace PragueMicroclimateProject.Tests.WebServices.Golemio.Mappers;

public class GolemioMicroclimateMapperTests
{
    /// <summary>
    /// Verifies grouping, mapping, and filtering of Golemio locations.
    /// </summary>
    [Fact]
    public void MapLocations_MixedSupportedPoints_GroupsMapsAndFiltersLocations()
    {
        // Arrange
        var points = new[]
        {
            new GolemioPoint
            {
                PointId = 101,
                LocationId = 10,
                PointNamed = "Point 101",
                LocationName = "Location 10",
                LocDescription = "Location description",
                LocSurface = MicroclimateSurfaceType.AsphaltConcrete,
                Lat = 50.08,
                Lng = 14.43,
                SensorPosition = MicroclimateSensorPositionType.PublicLighting,
                SensorPositionDetail = "Detailed position",
                Measures =
                [
                    new GolemioPointMeasure { Measure = MicroclimateMeasureType.AirTemperature200 },
                    new GolemioPointMeasure { Measure = MicroclimateMeasureType.Pressure200 }
                ]
            },
            new GolemioPoint
            {
                PointId = 102,
                LocationId = 10,
                PointNamed = "Point 102",
                LocationName = "Location 10",
                LocDescription = "Location description",
                LocSurface = MicroclimateSurfaceType.AsphaltConcrete,
                SensorPosition = MicroclimateSensorPositionType.Wall,
                Measures =
                [
                    new GolemioPointMeasure { Measure = MicroclimateMeasureType.AirHumidity200 }
                ]
            },
            new GolemioPoint
            {
                PointId = 103,
                LocationId = 10,
                Measures = null
            },
            new GolemioPoint
            {
                PointId = 201,
                LocationId = 20,
                LocationName = "Unsupported location",
                Measures =
                [
                    new GolemioPointMeasure { Measure = MicroclimateMeasureType.Pressure200 }
                ]
            }
        };

        // Act
        var result = GolemioMicroclimateMapper.MapLocations(points);

        // Assert
        var location = Assert.Single(result);
        Assert.Equal(10, location.Id);
        Assert.Equal("Location 10", location.Name);
        Assert.Equal("Location description", location.Description);
        Assert.Equal(SurfaceTypeCodelist.AsphaltConcrete, location.Surface);
        Assert.Collection(
            location.Points,
            firstPoint =>
            {
                Assert.Equal(101, firstPoint.Id);
                Assert.Equal("Point 101", firstPoint.Name);
                Assert.Equal("Detailed position", firstPoint.Description);
                Assert.Equal(SensorPositionTypeCodelist.PublicLighting, firstPoint.SensorPosition);
                Assert.Equal(50.08, firstPoint.Latitude);
                Assert.Equal(14.43, firstPoint.Longitude);
                Assert.Equal([MeasurementTypeCodelist.AirTemperature], firstPoint.MeasurementTypes);
            },
            secondPoint =>
            {
                Assert.Equal(102, secondPoint.Id);
                Assert.Equal(MicroclimateSensorPositionType.Wall, secondPoint.Description);
                Assert.Equal(SensorPositionTypeCodelist.Wall, secondPoint.SensorPosition);
                Assert.Equal([MeasurementTypeCodelist.AirHumidity], secondPoint.MeasurementTypes);
            });
    }

    /// <summary>
    /// Verifies mapping of all supported measurement properties.
    /// </summary>
    [Fact]
    public void MapMeasurements_ExternalMeasurement_MapsAllSupportedProperties()
    {
        // Arrange
        var timestamp = new DateTimeOffset(2026, 7, 15, 12, 30, 0, TimeSpan.FromHours(2));
        var measurements = new[]
        {
            new GolemioMeasurement
            {
                LocationId = 10,
                PointId = 101,
                MeasuredAt = timestamp,
                Measure = MicroclimateMeasureType.AirTemperature200,
                Unit = MicroclimateMeasureUnit.DegreesCelsius,
                Value = 24.75
            }
        };

        // Act
        var result = GolemioMicroclimateMapper.MapMeasurements(measurements);

        // Assert
        var measurement = Assert.Single(result);
        Assert.Equal(10, measurement.LocationId);
        Assert.Equal(101, measurement.PointId);
        Assert.Equal(timestamp, measurement.Timestamp);
        Assert.Equal(MeasurementTypeCodelist.AirTemperature, measurement.Type);
        Assert.Equal(MeasurementUnitCodelist.Celsius, measurement.Unit);
        Assert.Equal(24.75, measurement.Value);
    }
}
