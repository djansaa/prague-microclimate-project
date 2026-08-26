using PragueMicroclimateProject.Models.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Codelists;

namespace PragueMicroclimateProject.WebServices.Golemio.Mappers;

/// <summary>
/// Maps external Golemio sensor position types to internal sensor position type codelists.
/// </summary>
public static class SensorPositionTypeMapper
{
    /// <summary>
    /// Converts a Golemio sensor position type to an internal sensor position type code.
    /// </summary>
    public static string? ToInternal(string? golemioSensorPosition) => golemioSensorPosition?.Trim() switch
    {
        MicroclimateSensorPositionType.Lighting => SensorPositionTypeCodelist.Lighting,
        MicroclimateSensorPositionType.Wall => SensorPositionTypeCodelist.Wall,
        MicroclimateSensorPositionType.PublicLighting => SensorPositionTypeCodelist.PublicLighting,
        MicroclimateSensorPositionType.Pole => SensorPositionTypeCodelist.Pole,
        MicroclimateSensorPositionType.HighVoltagePowerLinePole => SensorPositionTypeCodelist.HighVoltagePowerLinePole,
        _ => null
    };
}
