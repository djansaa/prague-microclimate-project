namespace PragueMicroclimateProject.Models;

/// <summary>
/// Represents a microclimate location in Prague.
/// </summary>
public record Location
{
    /// <summary>
    /// Unique identifier of the location.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// Display name of the location.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Human-readable description of the location.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Surface type associated with the location.
    /// </summary>
    public string? Surface { get; set; }

    /// <summary>
    /// Measurement points belonging to the location.
    /// </summary>
    public List<Point> Points { get; set; } = [];
}
