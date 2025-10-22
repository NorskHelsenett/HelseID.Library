using HelseId.Library.ClientCredentials.Services;

namespace HelseId.Library.ClientCredentials;

public static class HelseIdHttpClientBuilderExtensions
{
    /// <summary>
    /// Configures the named HttpClient to be automatically setup to use DPoP tokens.
    /// Note that this method does not allow passing of organization numbers to HelseID
    /// so it is only useful for Single-Tenant clients or against APIs that does not
    /// require organization information. 
    /// </summary>
    /// <param name="httpClientBuilder"></param>
    /// <param name="scope">The scope to be requested</param>
    /// <returns></returns>
    public static IHttpClientBuilder AddHelseIdClientCredentials(this IHttpClientBuilder httpClientBuilder, string scope)
    {
        httpClientBuilder.AddHttpMessageHandler(sp =>
        {
            var clientCredentialsFlow = sp.GetService<IHelseIdClientCredentialsFlow>()!;
            var dpopProofCreator = sp.GetService<IDPoPProofCreatorForApiRequests>()!;
            return new HelseIdDPoPDelegatingHandler(clientCredentialsFlow, dpopProofCreator, scope);
        });

        return httpClientBuilder;
    }
}
