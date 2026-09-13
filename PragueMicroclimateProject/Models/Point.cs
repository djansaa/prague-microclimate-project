namespace PragueMicroclimateProject.Models;

/// <summary>
/// Represents a measurement point within a microclimate location.
/// </summary>
public record Point
{
    /// <summary>
    /// Unique identifier of the point.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Display name of the point.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Human-readable description of the point.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Physical placement or sensor position of the point.
    /// </summary>
    public string? SensorPosition { get; set; }

    /// <summary>
    /// Latitude of the point.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Longitude of the point.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Measurement types available for the point.
    /// </summary>
    public List<string> MeasurementTypes { get; set; } = [];
}
