using PragueMicroclimateProject.Models.Codelists;
using PragueMicroclimateProject.WebServices.Golemio.Codelists;

namespace PragueMicroclimateProject.WebServices.Golemio.Mappers;

/// <summary>
/// Maps external Golemio measure units to internal measurement unit codelists.
/// </summary>
public static class MeasureUnitMapper
{
    /// <summary>
    /// Converts a raw Golemio unit string to an internal measurement unit code.
    /// </summary>
    public static string ToInternal(string? golemioUnit) => golemioUnit switch
    {
        MicroclimateMeasureUnit.DegreesCelsius => MeasurementUnitCodelist.Celsius,
        MicroclimateMeasureUnit.Percent => MeasurementUnitCodelist.Percent,
        MicroclimateMeasureUnit.KilometersPerHour => MeasurementUnitCodelist.KilometersPerHour,
        _ => throw new ArgumentOutOfRangeException(nameof(golemioUnit), golemioUnit, "Unknown Golemio measure unit")
    };

    /// <summary>
    /// Converts an internal measurement unit code to the raw Golemio unit string.
    /// </summary>
    public static string ToExternal(string? measureUnit) => measureUnit switch
    {
        MeasurementUnitCodelist.Celsius => MicroclimateMeasureUnit.DegreesCelsius,
        MeasurementUnitCodelist.Percent => MicroclimateMeasureUnit.Percent,
        MeasurementUnitCodelist.KilometersPerHour => MicroclimateMeasureUnit.KilometersPerHour,
        _ => throw new ArgumentOutOfRangeException(nameof(measureUnit), measureUnit, "Unknown internal measure unit")
    };
}
