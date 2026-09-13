using Microsoft.Extensions.Logging.Abstractions;
using PragueMicroclimateProject.Models;
using PragueMicroclimateProject.Models.Codelists;
using PragueMicroclimateProject.Options;
using PragueMicroclimateProject.WebServices.Golemio;

namespace PragueMicroclimateProject.Tests.WebServices.Golemio;

public class GolemioServiceTests
{
    /// <summary>
    /// Verifies that a missing range start is rejected.
    /// </summary>
    [Fact]
    public void ValidateMeasurementRequest_MissingFrom_ThrowsArgumentException()
    {
        // Arrange
        var service = CreateService();
        var to = new DateTimeOffset(2026, 1, 31, 0, 0, 0, TimeSpan.Zero);

        // Act
        Action act = () => service.ValidateMeasurementRequest(null, null, to);

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("from", exception.ParamName);
    }

    /// <summary>
    /// Verifies that a missing range end is rejected.
    /// </summary>
    [Fact]
    public void ValidateMeasurementRequest_MissingTo_ThrowsArgumentException()
    {
        // Arrange
        var service = CreateService();
        var from = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        Action act = () => service.ValidateMeasurementRequest(null, from, null);

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("to", exception.ParamName);
    }

    /// <summary>
    /// Verifies that a reversed date range is rejected.
    /// </summary>
    [Fact]
    public void ValidateMeasurementRequest_FromAfterTo_ThrowsArgumentException()
    {
        // Arrange
        var service = CreateService();
        var from = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2026, 1, 31, 0, 0, 0, TimeSpan.Zero);

        // Act
        Action act = () => service.ValidateMeasurementRequest(null, from, to);

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Contains("less than or equal", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Verifies enforcement of the maximum calendar-month range.
    /// </summary>
    [Fact]
    public void ValidateMeasurementRequest_RangeExceedsMaximumCalendarMonths_ThrowsArgumentException()
    {
        // Arrange
        var service = CreateService(maxCalendarMonthsPerRequest: 3);
        var from = new DateTimeOffset(2025, 12, 31, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        Action act = () => service.ValidateMeasurementRequest(null, from, to);

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Contains("at most 3 calendar months", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Verifies that disabled measurement types are rejected.
    /// </summary>
    [Fact]
    public void ValidateMeasurementRequest_DisabledMeasure_ThrowsArgumentException()
    {
        // Arrange
        var service = CreateService(enabledMeasurementTypes: [MeasurementTypeCodelist.AirTemperature]);
        var from = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2026, 1, 31, 0, 0, 0, TimeSpan.Zero);

        // Act
        Action act = () => service.ValidateMeasurementRequest(MeasurementTypeCodelist.WindSpeed, from, to);

        // Assert
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("measure", exception.ParamName);
    }

    /// <summary>
    /// Verifies normalization of a valid measurement request.
    /// </summary>
    [Fact]
    public void ValidateMeasurementRequest_ValidTrimmedMeasure_ReturnsNormalizedRequest()
    {
        // Arrange
        var service = CreateService(enabledMeasurementTypes: [MeasurementTypeCodelist.AirTemperature]);
        var from = new DateTimeOffset(2026, 1, 1, 10, 15, 0, TimeSpan.FromHours(1));
        var to = new DateTimeOffset(2026, 2, 28, 18, 45, 0, TimeSpan.FromHours(1));

        // Act
        var result = service.ValidateMeasurementRequest($"  {MeasurementTypeCodelist.AirTemperature}  ", from, to);

        // Assert
        Assert.Equal(from, result.Start);
        Assert.Equal(to, result.End);
        Assert.Equal(MeasurementTypeCodelist.AirTemperature, result.Measure);
    }

    /// <summary>
    /// Verifies normalization of an omitted measurement type.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateMeasurementRequest_MissingMeasure_ReturnsNullMeasure(string? measure)
    {
        // Arrange
        var service = CreateService();
        var from = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2026, 1, 31, 0, 0, 0, TimeSpan.Zero);

        // Act
        var result = service.ValidateMeasurementRequest(measure, from, to);

        // Assert
        Assert.Null(result.Measure);
    }

    /// <summary>
    /// Verifies inclusive month enumeration across a year boundary.
    /// </summary>
    [Fact]
    public void GetMonthsBetween_RangeAcrossYearBoundary_ReturnsInclusiveMonths()
    {
        // Arrange
        var from = new DateTimeOffset(2025, 12, 31, 23, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2026, 2, 1, 1, 0, 0, TimeSpan.Zero);

        // Act
        var result = GolemioService.GetMonthsBetween(from, to);

        // Assert
        Assert.Equal(
            [new DateOnly(2025, 12, 1), new DateOnly(2026, 1, 1), new DateOnly(2026, 2, 1)],
            result);
    }

    /// <summary>
    /// Verifies leap-year month bounds with a time-zone offset.
    /// </summary>
    [Fact]
    public void GetMonthBounds_LeapYearMonth_ReturnsCompleteMonthWithOffset()
    {
        // Arrange
        var month = new DateOnly(2024, 2, 1);
        var offset = TimeSpan.FromHours(1);

        // Act
        var result = GolemioService.GetMonthBounds(month, offset);

        // Assert
        Assert.Equal(new DateTimeOffset(2024, 2, 1, 0, 0, 0, offset), result.Start);
        Assert.Equal(new DateTimeOffset(2024, 3, 1, 0, 0, 0, offset).AddTicks(-1), result.End);
    }

    /// <summary>
    /// Verifies hourly grouping, averaging, and chronological sorting.
    /// </summary>
    [Fact]
    public void AggregateMeasurementsByHour_MixedMeasurements_GroupsAveragesAndSorts()
    {
        // Arrange
        var offset = TimeSpan.FromHours(2);
        var measurements = new[]
        {
            CreateMeasurement(1, 1, new DateTimeOffset(2026, 7, 1, 10, 30, 0, offset), null),
            CreateMeasurement(1, 1, new DateTimeOffset(2026, 7, 1, 9, 50, 0, offset), 20.111),
            CreateMeasurement(1, 1, new DateTimeOffset(2026, 7, 1, 9, 5, 0, offset), 20.222),
            CreateMeasurement(1, 2, new DateTimeOffset(2026, 7, 1, 9, 10, 0, offset), 15),
            CreateMeasurement(1, 1, null, 99)
        };

        // Act
        var result = GolemioService.AggregateMeasurementsByHour(measurements);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(result.OrderBy(measurement => measurement.Timestamp), result);

        var averagedMeasurement = Assert.Single(result, measurement => measurement.PointId == 1 && measurement.Timestamp?.Hour == 9);
        Assert.Equal(new DateTimeOffset(2026, 7, 1, 9, 0, 0, offset), averagedMeasurement.Timestamp);
        Assert.Equal(20.17, averagedMeasurement.Value);

        var separatePointMeasurement = Assert.Single(result, measurement => measurement.PointId == 2);
        Assert.Equal(15, separatePointMeasurement.Value);

        var nullValueMeasurement = Assert.Single(result, measurement => measurement.Timestamp?.Hour == 10);
        Assert.Null(nullValueMeasurement.Value);
    }

    /// <summary>
    /// Verifies inclusive range filtering and chronological sorting.
    /// </summary>
    [Fact]
    public void FilterMeasurementsToRange_UnsortedMeasurements_ReturnsInclusiveSortedRange()
    {
        // Arrange
        var from = new DateTimeOffset(2026, 7, 1, 9, 0, 0, TimeSpan.Zero);
        var to = new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero);
        var measurements = new[]
        {
            CreateMeasurement(1, 1, to.AddTicks(1), 4),
            CreateMeasurement(1, 1, to, 3),
            CreateMeasurement(1, 1, from, 2),
            CreateMeasurement(1, 1, from.AddTicks(-1), 1),
            CreateMeasurement(1, 1, null, 5)
        };

        // Act
        var result = GolemioService.FilterMeasurementsToRange(measurements, from, to);

        // Assert
        Assert.Collection(
            result,
            first => Assert.Equal(from, first.Timestamp),
            second => Assert.Equal(to, second.Timestamp));
    }

    private static GolemioService CreateService(int maxCalendarMonthsPerRequest = 3, List<string>? enabledMeasurementTypes = null)
    {
        var options = Microsoft.Extensions.Options.Options.Create(new ApplicationOptions
        {
            MaxCalendarMonthsPerRequest = maxCalendarMonthsPerRequest,
            EnabledMeasurementTypes = enabledMeasurementTypes ?? []
        });

        return new GolemioService(null!, null!, NullLogger<GolemioService>.Instance, options);
    }

    private static Measurement CreateMeasurement(int locationId, int pointId, DateTimeOffset? timestamp, double? value)
    {
        return new Measurement
        {
            LocationId = locationId,
            PointId = pointId,
            Type = MeasurementTypeCodelist.AirTemperature,
            Unit = MeasurementUnitCodelist.Celsius,
            Timestamp = timestamp,
            Value = value
        };
    }
}
