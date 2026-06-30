using PragueMicroclimateProject.Models.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Codelists;

namespace PragueMicroclimateProject.WebServices.Golemio.Mappers;

/// <summary>
/// Mapping external Golemio measure types to internal measure types
/// </summary>
public static class MeasureTypeMapper
{
    /// <summary>
    /// Convert Golemio measure type to internal measure type
    /// </summary>
    public static string? ToInternal(string? golemioMeasure) => golemioMeasure switch
    {
        MicroclimateMeasureType.AirTemperature200 => MeasurementTypeCodelist.AirTemperature,
        MicroclimateMeasureType.AirHumidity200 => MeasurementTypeCodelist.AirHumidity,
        MicroclimateMeasureType.WindSpeed300 => MeasurementTypeCodelist.WindSpeed,
        // MicroclimateMeasureType.AirHumidity50
        // MicroclimateMeasureType.AirTemperature50
        // MicroclimateMeasureType.DendrometerCircumference200
        // MicroclimateMeasureType.DendrometerCircumferenceGain200
        // MicroclimateMeasureType.Precipitation300
        // MicroclimateMeasureType.Pressure200
        // MicroclimateMeasureType.Pressure50
        // MicroclimateMeasureType.SoilTemperatureMinus10
        // MicroclimateMeasureType.SoilTemperatureMinus30
        // MicroclimateMeasureType.SolarIrradiance200
        // MicroclimateMeasureType.SoilWaterPotentialMinus10
        // MicroclimateMeasureType.SoilWaterPotentialMinus30
        // MicroclimateMeasureType.WindDirection300
        // MicroclimateMeasureType.WindImpact300

        _ => throw new ArgumentOutOfRangeException(nameof(golemioMeasure), golemioMeasure, "Unknown Golemio measure type")
    };

    /// <summary>
    /// Convert internal measure type to Golemio measure type
    /// </summary>
    public static string? ToExternal(string? measureType) => measureType switch
    {
        MeasurementTypeCodelist.AirTemperature => MicroclimateMeasureType.AirTemperature200,
        MeasurementTypeCodelist.AirHumidity => MicroclimateMeasureType.AirHumidity200,
        MeasurementTypeCodelist.WindSpeed => MicroclimateMeasureType.WindSpeed300,

        _ => throw new ArgumentOutOfRangeException(nameof(measureType), measureType, "Unknown internal measure type")
    };
}
