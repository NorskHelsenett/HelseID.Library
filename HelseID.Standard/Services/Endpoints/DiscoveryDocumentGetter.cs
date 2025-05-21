using System.Net.Http.Json;
using System.Text.Json;
using HelseID.Standard.Configuration;
using HelseID.Standard.Interfaces.Endpoints;
using HelseID.Standard.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace HelseID.Standard.Services.Endpoints;

public class DiscoveryDocumentGetter : IDiscoveryDocumentGetter
{
    private const string DiscoveryDocumentKey = "DiscoveryDocument";
    private readonly IDistributedCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _discoveryUrl;

    public DiscoveryDocumentGetter(HelseIdConfiguration helseIdConfiguration, IDistributedCache cache, IHttpClientFactory httpClientFactory)
    {
        _discoveryUrl = helseIdConfiguration.MetadataUrl;
        _cache = cache;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<DiscoveryDocument> GetDiscoveryDocument()
    {
        var discoveryDocumentBytes = await _cache.GetAsync(DiscoveryDocumentKey);
        if (discoveryDocumentBytes == null || discoveryDocumentBytes.Length == 0)
        {
            return await UpdateCacheWithNewDocument();
        }
        
        var discoveryDocument = JsonSerializer.Deserialize<DiscoveryDocument>(discoveryDocumentBytes);
        if (discoveryDocument == null)
        {
            return await UpdateCacheWithNewDocument();
        }
        
        return discoveryDocument;
    }

    private async Task<DiscoveryDocument> UpdateCacheWithNewDocument()
    {
        var discoveryDocument = await CallTheMetadataUrl();
        var discoveryDocumentBytes = JsonSerializer.SerializeToUtf8Bytes(discoveryDocument);
        
        await _cache.SetAsync(DiscoveryDocumentKey, discoveryDocumentBytes);

        return discoveryDocument;
    }

    private async Task<DiscoveryDocument> CallTheMetadataUrl()
    {
        using var httpClient = _httpClientFactory.CreateClient();

        var discoveryDocumentResponse = await httpClient.GetAsync(_discoveryUrl);
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
