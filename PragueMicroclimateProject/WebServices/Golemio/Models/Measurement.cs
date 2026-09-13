using System.Text.Json.Serialization;

namespace PragueMicroclimateProject.WebServices.Golemio.Models;

/// <summary>
/// Microclimate measurement response model.
/// </summary>
public record Measurement
{
    /// <summary>
    /// PointId
    /// </summary>
    [JsonPropertyName("point_id")]
    public double? PointId { get; set; }

    /// <summary>
    /// LocationId
    /// </summary>
    [JsonPropertyName("location_id")]
    public double? LocationId { get; set; }

    /// <summary>
    /// MeasuredAt [example: "2022-08-21T17:30:00.000Z"]
    /// </summary>
    [JsonPropertyName("measured_at")]
    public DateTimeOffset? MeasuredAt { get; set; }

    /// <summary>
    /// Measure [example: "air_temp200"]
    /// </summary>
    [JsonPropertyName("measure")]
    public string? Measure { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    [JsonPropertyName("value")]
    public double? Value { get; set; }

    /// <summary>
    /// Unit [example: "°C"]
    /// </summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}
