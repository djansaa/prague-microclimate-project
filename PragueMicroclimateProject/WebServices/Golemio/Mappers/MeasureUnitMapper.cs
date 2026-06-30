using PragueMicroclimateProject.Models.Codelists;

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
        "\u00B0C" => MeasurementUnitCodelist.Celsius,
        "\u00C2\u00B0C" => MeasurementUnitCodelist.Celsius,
        "deg C" => MeasurementUnitCodelist.Celsius,
        "%" => MeasurementUnitCodelist.Percent,
        "km/h" => MeasurementUnitCodelist.KilometersPerHour,
        _ => throw new ArgumentOutOfRangeException(nameof(golemioUnit), golemioUnit, "Unknown Golemio measure unit")
    };

    /// <summary>
    /// Converts an internal measurement unit code to the raw Golemio unit string.
    /// </summary>
    public static string ToExternal(string? measureUnit) => measureUnit switch
    {
        MeasurementUnitCodelist.Celsius => "\u00B0C",
        MeasurementUnitCodelist.Percent => "%",
        MeasurementUnitCodelist.KilometersPerHour => "km/h",
        _ => throw new ArgumentOutOfRangeException(nameof(measureUnit), measureUnit, "Unknown internal measure unit")
    };
}
