using HelseID.Standard.Interfaces.Endpoints;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;

namespace HelseID.Standard.Services.Endpoints;

// This class encapsulates the 'GetDiscoveryDocumentAsync' method from
// the IdentityModel library, and caches the results
public class DiscoveryDocumentGetter : IDiscoveryDocumentGetter
{
    private const string DiscoveryDocumentKey = "DiscoveryDocument";

    private readonly IMemoryCache _memoryCache;
    private readonly string _stsUrl;

    public DiscoveryDocumentGetter(string stsUrl, IMemoryCache memoryCache)
    {
        _stsUrl = stsUrl;
        _memoryCache = memoryCache;
    }

    public async Task<DiscoveryDocumentResponse> GetDiscoveryDocument()
    {
        if (_memoryCache.TryGetValue(DiscoveryDocumentKey, out DiscoveryDocumentResponse? result))
        {
            return result!;
        }

        return await UpdateCacheWithNewDocument();
    }

    private async Task<DiscoveryDocumentResponse> UpdateCacheWithNewDocument()
    {
        var discoveryDocument = await CallTheMetadataUrl();

        _memoryCache.Set(DiscoveryDocumentKey, discoveryDocument);

        return discoveryDocument;
    }

    private async Task<DiscoveryDocumentResponse> CallTheMetadataUrl()
    {
        using var httpClient = SetupHttpClient();
        // This extension from the IdentityModel library calls the discovery document on the HelseID server
        var discoveryDocumentResponse = await httpClient.GetDiscoveryDocumentAsync(_stsUrl);
        if (discoveryDocumentResponse.IsError)
        {
            throw new Exception(discoveryDocumentResponse.Error);
        }
        return discoveryDocumentResponse;
    }

    protected virtual HttpClient SetupHttpClient()
    {
        return new HttpClient();
    }
}
