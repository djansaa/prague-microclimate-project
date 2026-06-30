using PragueMicroclimateProject.Models.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Codelists;

namespace PragueMicroclimateProject.WebServices.Golemio.Mappers;

/// <summary>
/// Mapping external Golemio measure units to internal measure units
/// </summary>
public static class MeasureUnitMapper
{
    /// <summary>
    /// Convert Golemio measure unit to internal measure unit
    /// </summary>
    public static string? ToInternal(string? golemioUnit) => golemioUnit switch
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

        _ => throw new ArgumentOutOfRangeException(nameof(golemioUnit), golemioUnit, "Unknown Golemio measure unit")
    };

    /// <summary>
    /// Convert internal measure unit to Golemio measure unit
    /// </summary>
    public static string? ToExternal(string? measureUnit) => measureUnit switch
    {
        MeasurementTypeCodelist.AirTemperature => MicroclimateMeasureType.AirTemperature200,
        MeasurementTypeCodelist.AirHumidity => MicroclimateMeasureType.AirHumidity200,
        MeasurementTypeCodelist.WindSpeed => MicroclimateMeasureType.WindSpeed300,

        _ => throw new ArgumentOutOfRangeException(nameof(measureUnit), measureUnit, "Unknown internal measure unit")
    };
}
