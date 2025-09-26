using HelseId.Library.Interfaces.Configuration;

namespace HelseId.Library.Services.Endpoints;

public class DiscoveryDocumentGetter : IDiscoveryDocumentGetter
{
    private readonly IDiscoveryDocumentCache _discoveryDocumentCache;
    private readonly IHelseIdConfigurationGetter _helseIdConfiguration;
    private readonly IHttpClientFactory _httpClientFactory;

    public DiscoveryDocumentGetter(
        IHelseIdConfigurationGetter helseIdConfiguration,
        IHttpClientFactory httpClientFactory,
        IDiscoveryDocumentCache discoveryDocumentCache)
    {
        _helseIdConfiguration = helseIdConfiguration;
        _httpClientFactory = httpClientFactory;
        _discoveryDocumentCache = discoveryDocumentCache;
    }

    public async Task<DiscoveryDocument> GetDiscoveryDocument()
    {
        var discoveryDocument = await _discoveryDocumentCache.GetDiscoveryDocument();
        
        if (discoveryDocument == null)
        {
            return await UpdateCacheWithNewDocument();
        }
        
        return discoveryDocument;
    }

    private async Task<DiscoveryDocument> UpdateCacheWithNewDocument()
    {
        var discoveryDocument = await CallTheMetadataUrl();
        await _discoveryDocumentCache.AddDiscoveryDocumentToCache(discoveryDocument);
        return discoveryDocument;
    }

    private async Task<DiscoveryDocument> CallTheMetadataUrl()
    {
        using var httpClient = _httpClientFactory.CreateClient();

        var configuration = await _helseIdConfiguration.GetConfiguration();
        
        var discoveryDocumentResponse = await httpClient.GetAsync(configuration.MetadataUrl);
        if (!discoveryDocumentResponse.IsSuccessStatusCode)
        {
            throw new Exception("Error getting discovery document");
        }
        return await GetDiscoveryDocumentFromResponse(discoveryDocumentResponse);
    }

    private static async Task<DiscoveryDocument> GetDiscoveryDocumentFromResponse(HttpResponseMessage discoveryDocumentResponse)
    {
        var discoveryDocument = await discoveryDocumentResponse.Content.ReadFromJsonAsync<DiscoveryDocument>();
        if (discoveryDocument == null)
        {
            throw new Exception("Did not get a discovery document");
        }

        return discoveryDocument;
    }
}
