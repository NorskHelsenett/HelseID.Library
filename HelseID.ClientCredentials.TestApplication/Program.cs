using HelseId.Library;
using HelseId.Library.Configuration;
using HelseId.Library.ClientCredentials;
using HelseId.Library.ClientCredentials.Interfaces;
using HelseId.Library.ExtensionMethods;
using HelseId.Library.Interfaces.JwtTokens;
using HelseId.Library.Models.DetailsFromClient;
using HelseId.Library.Selvbetjening.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HelseID.ClientCredentials.TestApplication;

sealed class Program
{
    static void Main(string[] args)
    {
        // Setup the Host that will contain the application
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        
        // Configure the HelseID Client
        var helseIdConfiguration = new HelseIdConfiguration
        {
            ClientId = "f2778e88-4c3d-44b5-a4ae-8ae8e6ca0692",
            Scope = "nhn:helseid-testapi/api nhn:selvbetjening/client",
            IssuerUri = "https://helseid-sts.test.nhn.no",
        };

        // Configure the library with the given configuration and a secret contained in a file
        // In this example we use the multi-tenant pattern
        builder.Services.AddHelseIdClientCredentials(helseIdConfiguration)
            .AddJwkFileForClientAuthentication("jwk.json")
            .AddHelseIdMultiTenant();
        
        // Register a service that will call HelseID
        builder.Services.AddHostedService<TestService>();

        // Run the application
        builder.Build().Run();
    }
}

/// <summary>
/// This service will run for the lifetime of the application.
/// It functions as a usage of the Client Credentials flow and the Dpop Proof Creator.
/// The IHostedService interface in a standard .NET method of running a service inside
/// a .NET application.
/// </summary>
public class TestService : IHostedService
{
    private readonly IHelseIdClientCredentialsFlow _helseIdClientCredentialsFlow;
    private readonly ISelvbetjeningSecretUpdater _selvbetjeningSecretUpdater;
    private readonly IDPoPProofCreatorForApiRequests _dPoPProofCreator;
    private readonly IHttpClientFactory _httpClientFactory;

    public TestService(
        IHelseIdClientCredentialsFlow helseIdClientCredentialsFlow,
        ISelvbetjeningSecretUpdater selvbetjeningSecretUpdater,
        IDPoPProofCreatorForApiRequests dPoPProofCreator,
        IHttpClientFactory httpClientFactory)
    {
        _helseIdClientCredentialsFlow = helseIdClientCredentialsFlow;
        _selvbetjeningSecretUpdater = selvbetjeningSecretUpdater;
        _dPoPProofCreator = dPoPProofCreator;
        _httpClientFactory = httpClientFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var organizationNumbers = new OrganizationNumbers
        {
            ParentOrganization = "994598759", // NHN
            ChildOrganization = "920773230" // NHN Bergen
        };

        var tokenResponse = await _helseIdClientCredentialsFlow.GetTokenResponseAsync(organizationNumbers);
        if (!tokenResponse.IsSuccessful(out var accessTokenResponse))
        {
            // Handle an error response from HelseID
            var errorResponse = tokenResponse.AsError();
            Console.WriteLine(errorResponse.Error + " " + errorResponse.ErrorDescription); 
            return;
        }
        
        // Create a DPoP proof for an API request
        // This can be automated, see our documentation for more examples
        var dpopProof = await _dPoPProofCreator.CreateDPoPProofForApiRequest(
            "https://api.example.com/api-endpoint", 
            "POST", 
            accessTokenResponse);

        // Build a http request to the API endpoint
        var apiRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://api.example.com/api-endpoint"),
        };

        // Set the Authorization- and DPoP-headers using an extension method from the library
        apiRequest.SetDPoPTokenAndProof(accessTokenResponse, dpopProof);

        var httpClient = _httpClientFactory.CreateClient();
        
        // Perform the request to the API
        var response = await httpClient.SendAsync(apiRequest, cancellationToken);
        
        // Do something with the response...
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
