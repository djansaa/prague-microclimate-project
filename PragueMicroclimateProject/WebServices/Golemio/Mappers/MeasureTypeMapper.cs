using PragueMicroclimateProject.Models.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Codelists;

namespace PragueMicroclimateProject.WebServices.Golemio.Mappers;

/// <summary>
/// Maps external Golemio measure types to internal measurement type codelists.
/// </summary>
public static class MeasureTypeMapper
{
    /// <summary>
    /// Converts a Golemio measure type to an internal measurement type code.
    /// </summary>
    public static string ToInternal(string? golemioMeasure) => golemioMeasure switch
    {
        MicroclimateMeasureType.AirTemperature200 => MeasurementTypeCodelist.AirTemperature,
        MicroclimateMeasureType.AirHumidity200 => MeasurementTypeCodelist.AirHumidity,
        MicroclimateMeasureType.WindSpeed300 => MeasurementTypeCodelist.WindSpeed,
        _ => throw new ArgumentOutOfRangeException(nameof(golemioMeasure), golemioMeasure, "Unknown Golemio measure type")
    };

    /// <summary>
    /// Converts an internal measurement type code to the primary Golemio measure type.
    /// </summary>
    public static string ToExternal(string? measureType) => measureType switch
    {
        MeasurementTypeCodelist.AirTemperature => MicroclimateMeasureType.AirTemperature200,
        MeasurementTypeCodelist.AirHumidity => MicroclimateMeasureType.AirHumidity200,
        MeasurementTypeCodelist.WindSpeed => MicroclimateMeasureType.WindSpeed300,
        _ => throw new ArgumentOutOfRangeException(nameof(measureType), measureType, "Unknown internal measure type")
    };
}
