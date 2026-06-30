namespace PragueMicroclimateProject.Models;

/// <summary>
/// Represents a normalized microclimate measurement.
/// </summary>
public class Measurement
{
    /// <summary>
    /// Identifier of the location the measurement belongs to.
    /// </summary>
    public int? LocationId { get; set; }

    /// <summary>
    /// Identifier of the point the measurement belongs to.
    /// </summary>
    public int? PointId { get; set; }

    /// <summary>
    /// Internal measurement type code.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Unit of the measured value.
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Date and time when the value was measured.
    /// </summary>
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// Numeric value of the measurement.
    /// </summary>
    public double? Value { get; set; }
}
