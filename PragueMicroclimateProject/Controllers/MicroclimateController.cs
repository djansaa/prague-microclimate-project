using Microsoft.AspNetCore.Mvc;
using PragueMicroclimateProject.WebServices.Golemio;
using PragueMicroclimateProject.WebServices.Golemio.Models;

namespace PragueMicroclimateProject.Controllers;

/// <summary>
/// MicroclimateController for handling microclimate-related API endpoints.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class MicroclimateController : ControllerBase
{
    private readonly GolemioService _golemioService;

    /// <summary>
    /// ctor
    /// </summary>
    public MicroclimateController(GolemioService golemioService)
    {
        _golemioService = golemioService;
    }

    /// <summary>
    /// Get ALL microclimate locations.
    /// </summary>
    [HttpGet("locations")]
    [ProducesResponseType(typeof(List<Location>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Location>>> GetAllLocations(CancellationToken cancellationToken = default)
    {
        var locations = await _golemioService.GetAllLocations(cancellationToken);
        return Ok(locations);
    }

    /// <summary>
    /// Get ALL microclimate locations and points.
    /// </summary>
    [HttpGet("locationsAndPoints")]
    [ProducesResponseType(typeof(List<Point3>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Point3>>> GetAllLocationsAndPoints(CancellationToken cancellationToken = default)
    {
        var points = await _golemioService.GetAllLocationAndPoints(cancellationToken);
        return Ok(points);
    }

    /// <summary>
    /// Get point measurements.
    /// </summary>
    [HttpGet("pointMeasurements")]
    [ProducesResponseType(typeof(List<Measurement>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Measurement>>> GetPointMeasurements([FromQuery] int? locationId = null, [FromQuery] int? pointId = null, [FromQuery] string? measure = null,
        [FromQuery] DateTimeOffset? from = null, [FromQuery] DateTimeOffset? to = null, CancellationToken cancellationToken = default)
    {
        var measurements = await _golemioService.GetPointMeasurements(locationId, pointId, measure, from, to, cancellationToken);
        return Ok(measurements);
    }

    /// <summary>
    /// Get point measurements.
    /// </summary>
    [HttpGet("pointMeasurementsParallel")]
    [ProducesResponseType(typeof(List<Measurement>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Measurement>>> GetPointMeasurementsParallel([FromQuery] int? locationId = null, [FromQuery] int? pointId = null, [FromQuery] string? measure = null,
        [FromQuery] DateTimeOffset? from = null, [FromQuery] DateTimeOffset? to = null, CancellationToken cancellationToken = default)
    {
        var measurements = await _golemioService.GetPointMeasurementsParallel(locationId, pointId, measure, from, to, cancellationToken);
        return Ok(measurements);
    }
}
