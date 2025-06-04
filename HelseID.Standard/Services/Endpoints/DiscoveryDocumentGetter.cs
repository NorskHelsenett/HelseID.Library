using System.Net.Http.Json;
using System.Text.Json;
using HelseID.Standard.Configuration;
using HelseID.Standard.Interfaces.Caching;
using HelseID.Standard.Interfaces.Endpoints;
using HelseID.Standard.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace HelseID.Standard.Services.Endpoints;

public class DiscoveryDocumentGetter : IDiscoveryDocumentGetter
{
    private const string DiscoveryDocumentKey = "DiscoveryDocument";
    private readonly IDiscoveryDocumentCache _discoveryDocumentCache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _discoveryUrl;

    public DiscoveryDocumentGetter(
        HelseIdConfiguration helseIdConfiguration,
        IHttpClientFactory httpClientFactory,
        IDiscoveryDocumentCache discoveryDocumentCache)
    {
        _discoveryUrl = helseIdConfiguration.MetadataUrl;
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
