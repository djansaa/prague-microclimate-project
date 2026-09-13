using PragueMicroclimateProject.Models.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Mappers;

namespace PragueMicroclimateProject.Tests.WebServices.Golemio.Mappers;

public class MeasureUnitMapperTests
{
    /// <summary>
    /// Verifies conversion of supported external measurement units.
    /// </summary>
    [Theory]
    [InlineData(MicroclimateMeasureUnit.DegreesCelsius, MeasurementUnitCodelist.Celsius)]
    [InlineData(MicroclimateMeasureUnit.Percent, MeasurementUnitCodelist.Percent)]
    [InlineData(MicroclimateMeasureUnit.KilometersPerHour, MeasurementUnitCodelist.KilometersPerHour)]
    public void ToInternal_SupportedUnit_ReturnsInternalCode(string externalUnit, string expected)
    {
        // Arrange
        var input = externalUnit;

        // Act
        var result = MeasureUnitMapper.ToInternal(input);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies conversion of supported internal measurement units.
    /// </summary>
    [Theory]
    [InlineData(MeasurementUnitCodelist.Celsius, MicroclimateMeasureUnit.DegreesCelsius)]
    [InlineData(MeasurementUnitCodelist.Percent, MicroclimateMeasureUnit.Percent)]
    [InlineData(MeasurementUnitCodelist.KilometersPerHour, MicroclimateMeasureUnit.KilometersPerHour)]
    public void ToExternal_SupportedUnit_ReturnsExternalCode(string internalUnit, string expected)
    {
        // Arrange
        var input = internalUnit;

        // Act
        var result = MeasureUnitMapper.ToExternal(input);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies rejection of unsupported external measurement units.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("unknown")]
    public void ToInternal_UnsupportedUnit_ThrowsArgumentOutOfRangeException(string? externalUnit)
    {
        // Arrange
        var input = externalUnit;

        // Act
        Action act = () => MeasureUnitMapper.ToInternal(input);

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(act);
        Assert.Equal("golemioUnit", exception.ParamName);
    }

    /// <summary>
    /// Verifies rejection of unsupported internal measurement units.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("UNKNOWN")]
    public void ToExternal_UnsupportedUnit_ThrowsArgumentOutOfRangeException(string? internalUnit)
    {
        // Arrange
        var input = internalUnit;

        // Act
        Action act = () => MeasureUnitMapper.ToExternal(input);

        // Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(act);
        Assert.Equal("measureUnit", exception.ParamName);
    }
}
