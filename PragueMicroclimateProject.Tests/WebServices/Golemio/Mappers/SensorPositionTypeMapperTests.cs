using PragueMicroclimateProject.Models.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Mappers;

namespace PragueMicroclimateProject.Tests.WebServices.Golemio.Mappers;

public class SensorPositionTypeMapperTests
{
    /// <summary>
    /// Verifies conversion of supported sensor positions.
    /// </summary>
    [Theory]
    [InlineData(MicroclimateSensorPositionType.Lighting, SensorPositionTypeCodelist.Lighting)]
    [InlineData(MicroclimateSensorPositionType.Wall, SensorPositionTypeCodelist.Wall)]
    [InlineData(MicroclimateSensorPositionType.PublicLighting, SensorPositionTypeCodelist.PublicLighting)]
    [InlineData(MicroclimateSensorPositionType.Pole, SensorPositionTypeCodelist.Pole)]
    [InlineData(MicroclimateSensorPositionType.HighVoltagePowerLinePole, SensorPositionTypeCodelist.HighVoltagePowerLinePole)]
    public void ToInternal_SupportedPosition_ReturnsInternalCode(string externalPosition, string expected)
    {
        // Arrange
        var input = externalPosition;

        // Act
        var result = SensorPositionTypeMapper.ToInternal(input);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies trimming of sensor-position whitespace.
    /// </summary>
    [Fact]
    public void ToInternal_PositionWithSurroundingWhitespace_ReturnsInternalCode()
    {
        // Arrange
        var input = $"  {MicroclimateSensorPositionType.PublicLighting}  ";

        // Act
        var result = SensorPositionTypeMapper.ToInternal(input);

        // Assert
        Assert.Equal(SensorPositionTypeCodelist.PublicLighting, result);
    }

    /// <summary>
    /// Verifies that unsupported sensor positions return null.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("unknown")]
    public void ToInternal_UnsupportedPosition_ReturnsNull(string? externalPosition)
    {
        // Arrange
        var input = externalPosition;

        // Act
        var result = SensorPositionTypeMapper.ToInternal(input);

        // Assert
        Assert.Null(result);
    }
}
