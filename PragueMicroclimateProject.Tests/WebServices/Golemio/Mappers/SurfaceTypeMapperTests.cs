using PragueMicroclimateProject.Models.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Mappers;

namespace PragueMicroclimateProject.Tests.WebServices.Golemio.Mappers;

public class SurfaceTypeMapperTests
{
    /// <summary>
    /// Verifies conversion of supported surface types.
    /// </summary>
    [Theory]
    [InlineData(MicroclimateSurfaceType.Asphalt, SurfaceTypeCodelist.Asphalt)]
    [InlineData(MicroclimateSurfaceType.AsphaltConcrete, SurfaceTypeCodelist.AsphaltConcrete)]
    [InlineData(MicroclimateSurfaceType.AsphaltPavingStones, SurfaceTypeCodelist.AsphaltPavingStones)]
    [InlineData(MicroclimateSurfaceType.AsphaltCompactedGravelGreenery, SurfaceTypeCodelist.AsphaltCompactedGravelGreenery)]
    [InlineData(MicroclimateSurfaceType.AsphaltGreenery, SurfaceTypeCodelist.AsphaltGreenery)]
    [InlineData(MicroclimateSurfaceType.PavingStones, SurfaceTypeCodelist.PavingStones)]
    [InlineData(MicroclimateSurfaceType.ArableLandGreenery, SurfaceTypeCodelist.ArableLandGreenery)]
    [InlineData(MicroclimateSurfaceType.Greenery, SurfaceTypeCodelist.Greenery)]
    [InlineData(MicroclimateSurfaceType.GreeneryCompactedGravel, SurfaceTypeCodelist.GreeneryCompactedGravel)]
    public void ToInternal_SupportedSurface_ReturnsInternalCode(string externalSurface, string expected)
    {
        // Arrange
        var input = externalSurface;

        // Act
        var result = SurfaceTypeMapper.ToInternal(input);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies trimming of surface-type whitespace.
    /// </summary>
    [Fact]
    public void ToInternal_SurfaceWithSurroundingWhitespace_ReturnsInternalCode()
    {
        // Arrange
        var input = $"  {MicroclimateSurfaceType.AsphaltConcrete}  ";

        // Act
        var result = SurfaceTypeMapper.ToInternal(input);

        // Assert
        Assert.Equal(SurfaceTypeCodelist.AsphaltConcrete, result);
    }

    /// <summary>
    /// Verifies that unsupported surface types return null.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("unknown")]
    public void ToInternal_UnsupportedSurface_ReturnsNull(string? externalSurface)
    {
        // Arrange
        var input = externalSurface;

        // Act
        var result = SurfaceTypeMapper.ToInternal(input);

        // Assert
        Assert.Null(result);
    }
}
