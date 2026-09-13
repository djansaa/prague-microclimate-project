using System.Text.Json.Serialization;

namespace PragueMicroclimateProject.WebServices.Golemio.Models;

/// <summary>
/// Microclimate point measure response model.
/// </summary>
public record Point3Measure
{
    /// <summary>
    /// Measure [example: "air_temp200"]
    /// </summary>
    [JsonPropertyName("measure")]
    public string? Measure { get; set; }

    /// <summary>
    /// MeasureCz [example: "Teplota vzduchu, 200 cm"]
    /// </summary>
    [JsonPropertyName("measure_cz")]
    public string? MeasureCz { get; set; }

    /// <summary>
    /// Unit [example: "°C"]
    /// </summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}
