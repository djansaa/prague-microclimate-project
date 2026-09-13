using System.Text.Json.Serialization;

namespace PragueMicroclimateProject.WebServices.Golemio.Models;

/// <summary>
/// Microclimate point response model.
/// </summary>
public record Point3
{
    /// <summary>
    /// PointId
    /// </summary>
    [JsonPropertyName("point_id")]
    public int PointId { get; set; }

    /// <summary>
    /// LocationId
    /// </summary>
    [JsonPropertyName("location_id")]
    public int LocationId { get; set; }

    /// <summary>
    /// PointNamed [example: "Pražská tržnice osvětlení"]
    /// </summary>
    [JsonPropertyName("point_named")]
    public string? PointNamed { get; set; }

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
    /// Lat
    /// </summary>
    [JsonPropertyName("lat")]
    public double? Lat { get; set; }

    /// <summary>
    /// Lng
    /// </summary>
    [JsonPropertyName("lng")]
    public double? Lng { get; set; }

    /// <summary>
    /// XJtsk
    /// </summary>
    [JsonPropertyName("x_jtsk")]
    public double? XJtsk { get; set; }

    /// <summary>
    /// YJtsk
    /// </summary>
    [JsonPropertyName("y_jtsk")]
    public double? YJtsk { get; set; }

    /// <summary>
    /// ElevationM
    /// </summary>
    [JsonPropertyName("elevation_m")]
    public double? ElevationM { get; set; }

    /// <summary>
    /// Measures
    /// </summary>
    [JsonPropertyName("measures")]
    public List<Point3Measure>? Measures { get; set; }

    /// <summary>
    /// SensorPosition [example: "veřejné osvětlení"]
    /// </summary>
    [JsonPropertyName("sensor_position")]
    public string? SensorPosition { get; set; }

    /// <summary>
    /// SensorPositionDetail [example: "veřejné osvětlení ve středu Tržnice"]
    /// </summary>
    [JsonPropertyName("sensor_position_detail")]
    public string? SensorPositionDetail { get; set; }
}
