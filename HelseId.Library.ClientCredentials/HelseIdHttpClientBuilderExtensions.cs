using HelseId.Library.ClientCredentials.Services;

namespace HelseId.Library.ClientCredentials;

public static class HelseIdHttpClientBuilderExtensions
{
    /// <summary>
    /// Configures the named HttpClient to be automatically setup to use DPoP tokens  
    /// </summary>
    /// <param name="httpClientBuilder"></param>
    /// <param name="scope"></param>
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
