using System.Net.Http.Json;
using HelseID.Standard.Configuration;
using HelseID.Standard.Interfaces.Endpoints;
using HelseID.Standard.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HelseID.Standard.Services.Endpoints;

// This class encapsulates the 'GetDiscoveryDocumentAsync' method from
// the IdentityModel library, and caches the results
public class DiscoveryDocumentGetter : IDiscoveryDocumentGetter
{
    private const string DiscoveryDocumentKey = "DiscoveryDocument";
    private readonly IMemoryCache _memoryCache;
    private readonly string _discoveryUrl;

    public DiscoveryDocumentGetter(string stsUrl, IMemoryCache memoryCache)
    {
        _discoveryUrl = stsUrl + "/.well-known/openid-configuration";
        _memoryCache = memoryCache;
    }

    public DiscoveryDocumentGetter(HelseIdConfiguration helseIdConfiguration, IMemoryCache memoryCache)
    {
        _discoveryUrl = helseIdConfiguration.StsUrl + "/.well-known/openid-configuration";
        _memoryCache = memoryCache;
    }

    public async Task<DiscoveryDocument> GetDiscoveryDocument()
    {
        if (_memoryCache.TryGetValue(DiscoveryDocumentKey, out DiscoveryDocument? result))
        {
            return result!;
        }

        return await UpdateCacheWithNewDocument();
    }

    private async Task<DiscoveryDocument> UpdateCacheWithNewDocument()
    {
        var discoveryDocument = await CallTheMetadataUrl();

        _memoryCache.Set(DiscoveryDocumentKey, discoveryDocument);

        return discoveryDocument;
    }

    private async Task<DiscoveryDocument> CallTheMetadataUrl()
    {
        using var httpClient = SetupHttpClient();
        // This extension from the IdentityModel library calls the discovery document on the HelseID server
        var discoveryDocumentResponse = await httpClient.GetAsync(_discoveryUrl);
        if (!discoveryDocumentResponse.IsSuccessStatusCode)
        {
            throw new Exception("Error getting discovery document");
        }
        return await GetDiscoveryDocumentFromResponse(discoveryDocumentResponse);
    }

    private static async Task<DiscoveryDocument> GetDiscoveryDocumentFromResponse(HttpResponseMessage discoveryDocumentResponse)
    {
        return await discoveryDocumentResponse.Content.ReadFromJsonAsync<DiscoveryDocument>();
    }


    protected virtual HttpClient SetupHttpClient()
    {
        return new HttpClient();
    }
}
