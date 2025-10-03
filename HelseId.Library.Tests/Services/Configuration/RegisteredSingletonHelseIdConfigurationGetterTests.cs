using HelseId.Library.Services.Configuration;

namespace HelseId.Library.Tests.Services.Configuration;

[TestFixture]
public class RegisteredSingletonHelseIdConfigurationGetterTests
{
    [Test]
    public async Task GetConfiguration_returns_instance_registered_at_constructor()
    {
        var configuration = new HelseIdConfiguration
        {
            ClientId = "client id",
            Scope = "scope",
            StsUrl = "sts url"
        };
        var configurationGetter = new RegisteredSingletonHelseIdConfigurationGetter(configuration);

        var returnValue = await configurationGetter.GetConfiguration();
        returnValue.Should().BeSameAs(configuration);
    }
}
