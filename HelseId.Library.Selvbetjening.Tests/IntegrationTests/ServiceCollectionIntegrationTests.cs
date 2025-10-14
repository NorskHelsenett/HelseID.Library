using FluentAssertions;
using HelseId.Library.ClientCredentials;
using HelseId.Library.Configuration;
using HelseId.Library.Selvbetjening.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HelseId.Library.Selvbetjening.Tests.IntegrationTests;

[TestFixture]
public class ServiceProviderIntegrationTests
{
    private IServiceProvider _serviceProvider = null!;

    [SetUp]
    public void SetUp()
    {
        var serviceCollection = new ServiceCollection();

        var config = new HelseIdConfiguration
        {
            ClientId = "client id",
            Scope = "scope",
            IssuerUri = "sts"
        };
        
        var privateKey = """
                         {
                             "kty": "EC",
                             "d": "gG_F23ZY-adEqha8TuYrfK8_ZeUM4nayaCcadUJSOd4",
                             "use": "sig",
                             "crv": "P-256",
                             "kid": "C2j1VuUe35Ro7Ft-SI8IsYCOoc5DJIpBAv7RF6bvt0Y",
                             "x": "CSBoKn1AviZgIC4k_OuqoycTgW9spfW0y_PwHLvlM8g",
                             "y": "xAy5uGp9Z7ahndFabA2RQ-BzltHobqPyPKaKe8De7XQ",
                             "alg": "ES256"
                         }
                         """;
        
        serviceCollection
            .AddHelseIdClientCredentials(config)
            .AddJwkForClientAuthentication(privateKey)
            .AddSelvbetjeningKeyRotation();
        
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Test]
    public void GetService_does_not_throw_for_selvbetjening_secret_updater()
    {
        Action getService = () => _serviceProvider.GetService<ISelvbetjeningSecretUpdater>();

        getService.Should().NotThrow<Exception>();
    }
}
