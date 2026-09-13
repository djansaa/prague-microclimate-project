using System.Net;
using System.Text;

namespace PragueMicroclimateProject.Tests.TestInfrastructure;

internal sealed class RecordingHttpMessageHandler : HttpMessageHandler
{
    private readonly string _responseJson;

    public RecordingHttpMessageHandler(string responseJson = "[]")
    {
        _responseJson = responseJson;
    }

    public List<HttpRequestMessage> Requests { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_responseJson, Encoding.UTF8, "application/json"),
            RequestMessage = request
        };

        return Task.FromResult(response);
    }
}
