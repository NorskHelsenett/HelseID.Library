using RichardSzalay.MockHttp;

namespace HelseId.Standard.Tests.Mocks;

public class HttpClientFactoryMock : IHttpClientFactory
{
    public int RequestCount { get; set; }
    
    private readonly MockHttpMessageHandlerWithCount _httpMessageHandler;

    public HttpClientFactoryMock(MockHttpMessageHandlerWithCount httpMessageHandler)
    {
        _httpMessageHandler = httpMessageHandler;
        _httpMessageHandler.HttpClientFactoryMock = this;
    }
    
    public HttpClient CreateClient(string name)
    {
        return new HttpClient(_httpMessageHandler);
    }
}

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
