using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;
using PragueMicroclimateProject.Tests.TestInfrastructure;
using PragueMicroclimateProject.WebServices.Golemio.Clients;

namespace PragueMicroclimateProject.Tests.WebServices.Golemio.Clients;

public class GolemioClientTests
{
    /// <summary>
    /// Verifies the locations endpoint without an optional identifier.
    /// </summary>
    [Fact]
    public async Task GetMicroclimateLocationsAsync_WithoutLocationId_SendsBaseEndpoint()
    {
        // Arrange
        using var handler = new RecordingHttpMessageHandler();
        using var httpClient = CreateHttpClient(handler);
        var client = new GolemioClient(httpClient);

        // Act
        var result = await client.GetMicroclimateLocationsAsync();

        // Assert
        var request = Assert.Single(handler.Requests);
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Equal("/v2/microclimate/locations", request.RequestUri?.AbsolutePath);
        Assert.Empty(QueryHelpers.ParseQuery(request.RequestUri?.Query));
        Assert.Empty(Assert.IsType<List<PragueMicroclimateProject.WebServices.Golemio.Models.Location>>(result));
    }

    /// <summary>
    /// Verifies the locations endpoint location query parameter.
    /// </summary>
    [Fact]
    public async Task GetMicroclimateLocationsAsync_WithLocationId_AddsLocationQueryParameter()
    {
        // Arrange
        using var handler = new RecordingHttpMessageHandler();
        using var httpClient = CreateHttpClient(handler);
        var client = new GolemioClient(httpClient);

        // Act
        await client.GetMicroclimateLocationsAsync(42);

        // Assert
        var request = Assert.Single(handler.Requests);
        var query = QueryHelpers.ParseQuery(request.RequestUri?.Query);
        Assert.Equal("42", query["locationId"].ToString());
    }

    /// <summary>
    /// Verifies both identifier parameters on the points endpoint.
    /// </summary>
    [Fact]
    public async Task GetMicroclimatePointsAsync_WithBothIds_AddsAllQueryParameters()
    {
        // Arrange
        using var handler = new RecordingHttpMessageHandler();
        using var httpClient = CreateHttpClient(handler);
        var client = new GolemioClient(httpClient);

        // Act
        await client.GetMicroclimatePointsAsync(locationId: 12, pointId: 34);

        // Assert
        var request = Assert.Single(handler.Requests);
        Assert.Equal("/v2/microclimate/points", request.RequestUri?.AbsolutePath);
        var query = QueryHelpers.ParseQuery(request.RequestUri?.Query);
        Assert.Equal("12", query["locationId"].ToString());
        Assert.Equal("34", query["pointId"].ToString());
    }

    /// <summary>
    /// Verifies all measurement filters and invariant formatting.
    /// </summary>
    [Fact]
    public async Task GetMicroclimateMeasurementsAsync_WithAllFilters_AddsInvariantQueryParameters()
    {
        // Arrange
        using var handler = new RecordingHttpMessageHandler();
        using var httpClient = CreateHttpClient(handler);
        var client = new GolemioClient(httpClient);
        var from = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.FromHours(1));
        var to = new DateTimeOffset(2026, 2, 3, 4, 5, 6, TimeSpan.FromHours(1));

        // Act
        await client.GetMicroclimateMeasurementsAsync(12, 34, "air_temp200", from, to);

        // Assert
        var request = Assert.Single(handler.Requests);
        Assert.Equal("/v2/microclimate/measurements", request.RequestUri?.AbsolutePath);
        var query = QueryHelpers.ParseQuery(request.RequestUri?.Query);
        Assert.Equal("12", query["locationId"].ToString());
        Assert.Equal("34", query["pointId"].ToString());
        Assert.Equal("air_temp200", query["measure"].ToString());
        Assert.Equal(from.ToString("O", CultureInfo.InvariantCulture), query["from"].ToString());
        Assert.Equal(to.ToString("O", CultureInfo.InvariantCulture), query["to"].ToString());
    }

    /// <summary>
    /// Verifies omission of an empty measurement query parameter.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetMicroclimateMeasurementsAsync_MissingMeasure_OmitsMeasureQueryParameter(string? measure)
    {
        // Arrange
        using var handler = new RecordingHttpMessageHandler();
        using var httpClient = CreateHttpClient(handler);
        var client = new GolemioClient(httpClient);

        // Act
        await client.GetMicroclimateMeasurementsAsync(measure: measure);

        // Assert
        var request = Assert.Single(handler.Requests);
        var query = QueryHelpers.ParseQuery(request.RequestUri?.Query);
        Assert.False(query.ContainsKey("measure"));
    }

    private static HttpClient CreateHttpClient(HttpMessageHandler handler)
    {
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test")
        };
    }
}
