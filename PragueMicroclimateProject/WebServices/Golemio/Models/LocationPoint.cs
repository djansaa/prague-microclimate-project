using System.Text.Json.Serialization;

namespace PragueMicroclimateProject.WebServices.Golemio.Models;

/// <summary>
/// Microclimate location point response model.
/// </summary>
public class LocationPoint
{
    /// <summary>
    /// PointId
    /// </summary>
    [JsonPropertyName("point_id")]
    public int PointId { get; set; }

    /// <summary>
    /// PointName [example: "Pražská tržnice osvětlení"]
    /// </summary>
    [JsonPropertyName("point_name")]
    public string? PointName { get; set; }
}
