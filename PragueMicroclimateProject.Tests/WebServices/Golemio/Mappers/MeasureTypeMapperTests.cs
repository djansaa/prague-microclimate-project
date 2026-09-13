using PragueMicroclimateProject.Models.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Mappers;

namespace PragueMicroclimateProject.Tests.WebServices.Golemio.Mappers;

public class MeasureTypeMapperTests
{
    /// <summary>
    /// Verifies conversion of supported external measurement types.
    /// </summary>
    [Theory]
    [InlineData(MicroclimateMeasureType.AirTemperature200, MeasurementTypeCodelist.AirTemperature)]
    [InlineData(MicroclimateMeasureType.AirHumidity200, MeasurementTypeCodelist.AirHumidity)]
    [InlineData(MicroclimateMeasureType.WindSpeed300, MeasurementTypeCodelist.WindSpeed)]
    public void ToInternal_SupportedMeasure_ReturnsInternalCode(string externalMeasure, string expected)
    {
        // Arrange
        var input = externalMeasure;

        // Act
        var result = MeasureTypeMapper.ToInternal(input);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies conversion of supported internal measurement types.
    /// </summary>
    [Theory]
    [InlineData(MeasurementTypeCodelist.AirTemperature, MicroclimateMeasureType.AirTemperature200)]
    [InlineData(MeasurementTypeCodelist.AirHumidity, MicroclimateMeasureType.AirHumidity200)]
    [InlineData(MeasurementTypeCodelist.WindSpeed, MicroclimateMeasureType.WindSpeed300)]
    public void ToExternal_SupportedMeasure_ReturnsExternalCode(string internalMeasure, string expected)
    {
        // Arrange
        var input = internalMeasure;

        // Act
        var result = MeasureTypeMapper.ToExternal(input);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies that unsupported external measurement types return null.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("unknown")]
    public void ToInternal_UnsupportedMeasure_ReturnsNull(string? externalMeasure)
    {
        // Arrange
        var input = externalMeasure;

        // Act
        var result = MeasureTypeMapper.ToInternal(input);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that unsupported internal measurement types return null.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("UNKNOWN")]
    public void ToExternal_UnsupportedMeasure_ReturnsNull(string? internalMeasure)
    {
        // Arrange
        var input = internalMeasure;

        // Act
        var result = MeasureTypeMapper.ToExternal(input);

        // Assert
        Assert.Null(result);
    }
}
