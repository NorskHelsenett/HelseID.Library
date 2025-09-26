using Microsoft.Extensions.DependencyInjection;

namespace HelseId.Library;

public sealed class HelseIdBuilder : IHelseIdBuilder
{
    public HelseIdBuilder(IServiceCollection services)
    {
        Services = services;
        AddHelseIdBasics();
    }

    public IServiceCollection Services { get; private set; }
    
    private void AddHelseIdBasics()
    {
        Services.AddSingleton<IDPoPProofCreator, DPoPProofCreator>();
        Services.AddSingleton<IHelseIdEndpointsDiscoverer, HelseIdEndpointsDiscoverer>();
        Services.AddSingleton<ISigningTokenCreator, SigningTokenCreator>();
        Services.AddSingleton<IDiscoveryDocumentGetter, DiscoveryDocumentGetter>();
        Services.AddSingleton<IAssertionDetailsCreator, AssertionDetailsCreator>();
        Services.AddSingleton<IStructuredClaimsCreator, OrganizationNumberCreatorForSingleTenantClient>();
        Services.AddSingleton(TimeProvider.System);
        Services.AddHttpClient();
    }
        
    public void RemoveServiceRegistrations<TService>()
    {
        var existingServiceRegistration = Services.Where(s => s.ServiceType == typeof(TService)).ToArray();
        foreach (var serviceRegistration in existingServiceRegistration)
        {
            Services.Remove(serviceRegistration);
        }
    }
}
