namespace HelseId.Library.Tests.Mocks;

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
