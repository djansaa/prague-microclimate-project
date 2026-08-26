using PragueMicroclimateProject.Models.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Codelists;

namespace PragueMicroclimateProject.WebServices.Golemio.Mappers;

/// <summary>
/// Maps external Golemio surface types to internal surface type codelists.
/// </summary>
public static class SurfaceTypeMapper
{
    /// <summary>
    /// Converts a Golemio surface type to an internal surface type code.
    /// </summary>
    public static string? ToInternal(string? golemioSurface) => golemioSurface?.Trim() switch
    {
        MicroclimateSurfaceType.Asphalt => SurfaceTypeCodelist.Asphalt,
        MicroclimateSurfaceType.AsphaltConcrete => SurfaceTypeCodelist.AsphaltConcrete,
        MicroclimateSurfaceType.AsphaltPavingStones => SurfaceTypeCodelist.AsphaltPavingStones,
        MicroclimateSurfaceType.AsphaltCompactedGravelGreenery => SurfaceTypeCodelist.AsphaltCompactedGravelGreenery,
        MicroclimateSurfaceType.AsphaltGreenery => SurfaceTypeCodelist.AsphaltGreenery,
        MicroclimateSurfaceType.PavingStones => SurfaceTypeCodelist.PavingStones,
        MicroclimateSurfaceType.ArableLandGreenery => SurfaceTypeCodelist.ArableLandGreenery,
        MicroclimateSurfaceType.Greenery => SurfaceTypeCodelist.Greenery,
        MicroclimateSurfaceType.GreeneryCompactedGravel => SurfaceTypeCodelist.GreeneryCompactedGravel,
        _ => null
    };
}
