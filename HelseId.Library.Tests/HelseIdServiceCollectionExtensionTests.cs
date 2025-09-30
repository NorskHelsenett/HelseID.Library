using HelseId.Library.Interfaces.Configuration;
using Helseid.Library.SharedTests;

namespace HelseId.Library.Tests;

[TestFixture]
public class HelseIdServiceCollectionExtensionTests
{
    private ServiceCollection _serviceCollection = null!;
    private HelseIdConfiguration _config = null!;

    [SetUp]
    public void SetUp()
    {
        _serviceCollection = new ServiceCollection();
        _config = new HelseIdConfiguration("client id", "scope", "sts"); 
    }

    [Test]
    public void AddInMemoryHelseIdCaching_registers_expected_services()
    {
        _serviceCollection.AddHelseIdClientCredentials(_config).AddHelseIdInMemoryCaching();

        _serviceCollection.EnsureSingletonRegistration<ITokenCache, InMemoryTokenCache>();
        _serviceCollection.EnsureSingletonRegistration<IDiscoveryDocumentCache, InMemoryDiscoveryDocumentCache>();
    }

    [Test]
    public void AddDistributedHelseIdCaching_registers_expected_services()
    {
        _serviceCollection.AddHelseIdClientCredentials(_config).AddHelseIdDistributedCaching();

        _serviceCollection.EnsureSingletonRegistration<ITokenCache, DistributedTokenCache>();
        _serviceCollection.EnsureSingletonRegistration<IDiscoveryDocumentCache, DistributedDiscoveryDocumentCache>();
    }

    [Test]
    public void AddHelseIdConfigurationGetter_registers_supplied_instance_as_singleton()
    {
        var configurationGetter = new TestConfigurationGetter();
        _serviceCollection.AddHelseIdClientCredentials().AddHelseIdConfigurationGetter(configurationGetter);
        
        _serviceCollection.EnsureSingletonRegistration<IHelseIdConfigurationGetter, TestConfigurationGetter>(configurationGetter);
    }
    
    private sealed class TestConfigurationGetter : IHelseIdConfigurationGetter
    {
        public Task<HelseIdConfiguration> GetConfiguration()
        {
            throw new NotImplementedException();
        }
    }
}
