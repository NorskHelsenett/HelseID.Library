using System.Security.Cryptography.X509Certificates;
using Helseid.Library.SharedTests;
using HelseId.Library.Tests.Configuration;

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
        _config = new HelseIdConfiguration
        {
            ClientId = "client id",
            Scope = "scope",
            IssuerUri = "sts"
        };
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
    public void AddX509CertificateForForClientAuthentication_throws_exception_when_private_key_from_certificate_is_unreadable()
    {
        var certificate = X509CertificateGenerator.GenerateSelfSignedCertificate(
            "Self signed with unavailable private key",
            X509KeyUsageFlags.NonRepudiation, onlyPublicKey: true);

        Func<IHelseIdBuilder> addCertificate = () => _serviceCollection.AddHelseIdClientCredentials(_config)
            .AddX509CertificateForForClientAuthentication(certificate, "RS256");

        addCertificate.Should().
            Throw<ArgumentException>()
            .Where(ae => 
                ae.ParamName == "certificate" &&
                ae.Message.StartsWith("There is no private key available in the certificate with thumbprint"));
    }
    
    [Test]
    public void AddX509CertificateForForClientAuthentication_does_throw_exception_when_private_key_from_certificate_is_readable()
    {
        var certificate = X509CertificateGenerator.GenerateSelfSignedCertificate(
            "Self signed with unavailable private key",
            X509KeyUsageFlags.NonRepudiation, onlyPublicKey: false);

        Func<IHelseIdBuilder> addCertificate = () => _serviceCollection.AddHelseIdClientCredentials(_config)
            .AddX509CertificateForForClientAuthentication(certificate, "RS256");

        addCertificate.Should().NotThrow();
    }
}
