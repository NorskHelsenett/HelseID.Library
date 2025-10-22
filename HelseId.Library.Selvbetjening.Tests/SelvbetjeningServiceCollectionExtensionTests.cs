using HelseId.Library.ClientCredentials;
using HelseId.Library.Configuration;
using HelseId.Library.Selvbetjening.Interfaces;
using HelseId.Library.Selvbetjening.Services;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Helseid.Library.SharedTests;

namespace HelseId.Library.Selvbetjening.Tests;

[TestFixture]
public class SelvbetjeningServiceCollectionExtensionTests
{
    private ServiceCollection _serviceCollection = null!;
    private HelseIdConfiguration _config = null!;
    private IHelseIdBuilder _helseIdBuilder = null!;

    [SetUp]
    public void SetUp()
    {
        _serviceCollection = new ServiceCollection();
        _config = new HelseIdConfiguration
        {
            ClientId = "client id",
            Scope = "scope",
            IssuerUri = "sts"
        };

        _helseIdBuilder = _serviceCollection.AddHelseIdClientCredentials(_config);
    }
    
    [Test]
    public void AddInMemoryHelseIdCaching_registers_expected_services()
    {
        _helseIdBuilder.AddSelvbetjeningKeyRotation();

        _serviceCollection.EnsureSingletonRegistration<ISelvbetjeningSecretUpdater, SelvbetjeningSecretUpdater>();
        _serviceCollection.EnsureSingletonRegistration<IKeyManagementService, KeyManagementService>();
        _serviceCollection.EnsureSingletonRegistration<IClientSecretEndpoint, ClientSecretEndpoint>();
    }
}
