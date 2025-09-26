namespace HelseId.Library.Tests.Mocks;

public class MockHttpMessageHandlerWithCount : MockHttpMessageHandler
{
    public HttpClientFactoryMock? HttpClientFactoryMock { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (HttpClientFactoryMock != null)
        {
            HttpClientFactoryMock.RequestCount++;
        }
        return base.SendAsync(request, cancellationToken);
    }
}
