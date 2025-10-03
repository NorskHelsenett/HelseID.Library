using HelseId.Library;
using HelseId.Library.Configuration;
using HelseId.Library.ClientCredentials;
using HelseId.Library.ClientCredentials.Interfaces;
using HelseId.Library.ExtensionMethods;
using HelseId.Library.Models;
using HelseId.Library.Models.DetailsFromClient;
using HelseId.Library.Selvbetjening;
using HelseId.Library.Selvbetjening.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HelseID.ClientCredentials.TestApplication;

sealed class Program
{
    static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        var helseIdConfiguration = new HelseIdConfiguration
        {
            ClientId = "f2778e88-4c3d-44b5-a4ae-8ae8e6ca0692",
            Scope = "nhn:helseid-testapi/api nhn:selvbetjening/client",
            StsUrl = "https://helseid-sts.test.nhn.no",
        };

        builder.Services.AddHelseIdClientCredentials(helseIdConfiguration)
            .AddSelvbetjeningKeyRotation()
            .AddJwkFileForClientAuthentication("jwk.json")
            .AddHelseIdMultiTenant();
    
        builder.Services.AddHostedService<TestService>();

        IHost host = builder.Build();
        host.Run();
    }
}

public class TestService : IHostedService
{
    private readonly IHelseIdClientCredentialsFlow _helseIdClientCredentialsFlow;
    private readonly ISelvbetjeningSecretUpdater _selvbetjeningSecretUpdater;

    public TestService(IHelseIdClientCredentialsFlow helseIdClientCredentialsFlow, ISelvbetjeningSecretUpdater selvbetjeningSecretUpdater)
    {
        _helseIdClientCredentialsFlow = helseIdClientCredentialsFlow;
        _selvbetjeningSecretUpdater = selvbetjeningSecretUpdater;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var organizationNumbersBergen = new OrganizationNumbers
        {
            ParentOrganization = "994598759", // NHN
            ChildOrganization = "920773230" // NHN Bergen
        };
        
        var organizationNumbersTrondheim = new OrganizationNumbers
        {
            ParentOrganization = "994598759", // NHN
            ChildOrganization = "987402105" // NHN Trondheim
        };

        var tokenResponseBergen = await _helseIdClientCredentialsFlow.GetTokenResponseAsync(organizationNumbersBergen);
        if (tokenResponseBergen.IsSuccessful(out var accessTokenResponse))
        {
            Console.WriteLine(accessTokenResponse.AccessToken);    
        }
        else
        {
            var errorResponse = tokenResponseBergen.AsError();
            Console.WriteLine(errorResponse.Error + " " + errorResponse.ErrorDescription);
        }
        
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
    
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        await _selvbetjeningSecretUpdater.UpdateClientSecret();
        await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
        
        var tokenResponseTrondheim = await _helseIdClientCredentialsFlow.GetTokenResponseAsync(organizationNumbersTrondheim);
        Console.WriteLine(((AccessTokenResponse)tokenResponseTrondheim).AccessToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
} 
