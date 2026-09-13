using System.Text.Json.Serialization;

namespace PragueMicroclimateProject.WebServices.Golemio.Models;

/// <summary>
/// Microclimate location response model.
/// </summary>
public record Location
{
    /// <summary>
    /// LocationId
    /// </summary>
    [JsonPropertyName("location_id")]
    public int LocationId { get; set; }

    /// <summary>
    /// LocationName [example: "Pražská Holešovická tržnice"]
    /// </summary>
    [JsonPropertyName("location")]
    public string? LocationName { get; set; }

    /// <summary>
    /// LocDescription [example: "areál bez zeleně"]
    /// </summary>
    [JsonPropertyName("loc_description")]
    public string? LocDescription { get; set; }

    /// <summary>
    /// LocOrientation [example: "východ-západ"]
    /// </summary>
    [JsonPropertyName("loc_orientation")]
    public string? LocOrientation { get; set; }

    /// <summary>
    /// LocSurface [example: "asfalt/beton"]
    /// </summary>
    [JsonPropertyName("loc_surface")]
    public string? LocSurface { get; set; }

    /// <summary>
    /// Address [example: "Pražská tržnice"]
    /// </summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>
    /// Points
    /// </summary>
    [JsonPropertyName("points")]
    public List<LocationPoint>? Points { get; set; }
}
