using PragueMicroclimateProject.Tests.TestInfrastructure;
using PragueMicroclimateProject.WebServices.Golemio.Handlers;
using PragueMicroclimateProject.WebServices.Golemio.Options;

namespace PragueMicroclimateProject.Tests.WebServices.Golemio.Handlers;

public class GolemioAuthenticationHandlerTests
{
    private const string ApiKeyHeaderName = "X-Access-Token";

    /// <summary>
    /// Verifies that a configured API key is added to requests.
    /// </summary>
    [Fact]
    public async Task SendAsync_ConfiguredApiKey_AddsApiKeyHeader()
    {
        // Arrange
        using var innerHandler = new RecordingHttpMessageHandler();
        using var authenticationHandler = CreateAuthenticationHandler("configured-key", innerHandler);
        using var client = new HttpClient(authenticationHandler);

        // Act
        await client.GetAsync("https://example.test/resource");

        // Assert
        var request = Assert.Single(innerHandler.Requests);
        Assert.Equal("configured-key", Assert.Single(request.Headers.GetValues(ApiKeyHeaderName)));
    }

    /// <summary>
    /// Verifies that an existing API key header is preserved.
    /// </summary>
    [Fact]
    public async Task SendAsync_ExistingApiKey_PreservesExistingHeader()
    {
        // Arrange
        using var innerHandler = new RecordingHttpMessageHandler();
        using var authenticationHandler = CreateAuthenticationHandler("configured-key", innerHandler);
        using var client = new HttpClient(authenticationHandler);
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/resource");
        request.Headers.Add(ApiKeyHeaderName, "existing-key");

        // Act
        await client.SendAsync(request);

        // Assert
        var forwardedRequest = Assert.Single(innerHandler.Requests);
        Assert.Equal("existing-key", Assert.Single(forwardedRequest.Headers.GetValues(ApiKeyHeaderName)));
    }

    /// <summary>
    /// Verifies that an empty API key does not add a header.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task SendAsync_MissingApiKey_DoesNotAddApiKeyHeader(string? apiKey)
    {
        // Arrange
        using var innerHandler = new RecordingHttpMessageHandler();
        using var authenticationHandler = CreateAuthenticationHandler(apiKey, innerHandler);
        using var client = new HttpClient(authenticationHandler);

        // Act
        await client.GetAsync("https://example.test/resource");

        // Assert
        var request = Assert.Single(innerHandler.Requests);
        Assert.False(request.Headers.Contains(ApiKeyHeaderName));
    }

    private static GolemioAuthenticationHandler CreateAuthenticationHandler(string? apiKey, HttpMessageHandler innerHandler)
    {
        var options = Microsoft.Extensions.Options.Options.Create(new GolemioOptions
        {
            ApiKey = apiKey!,
            BaseUrl = "https://example.test",
            RateLimit = new RateLimitOptions()
        });

        return new GolemioAuthenticationHandler(options)
        {
            InnerHandler = innerHandler
        };
    }
}
