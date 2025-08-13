using HelseId.Library.Services.Configuration;

namespace HelseId.Library.Tests.Services.Configuration;

[TestFixture]
public class RegisteredSingletonHelseIdConfigurationGetterTests
{
    private const string GeneralPrivateEcKey = """
                                                 {
                                                     "kty": "EC",
                                                     "d": "PTQlsiXQ-PU_aeG1cSXZmEtm_rJH7Q5lEtqn9hP-SOlNHurT3vpM6gMy28h59G8u",
                                                     "use": "sig",
                                                     "crv": "P-384",
                                                     "x": "_fwQ_E2rqeBOQ0YYzQBCvZNK60-n4PUG7cbJelBkuAbfEqmnaMHNUsReIsnE3432",
                                                     "y": "xbuUzn7GpWq7JuKgrY_QxskViWPyDk_MoIef5JXXPlWkdB24cQLVgm-Jgz8NOblZ",
                                                     "alg": "ES384",
                                                     "kid": "kidvalue"
                                                 }
                                                 """;
    
    [Test]
    public async Task GetConfiguration_returns_instance_registered_at_constructor()
    {
        var configuration = HelseIdConfiguration.ConfigurationForJsonWebKey(GeneralPrivateEcKey,
            "client id",
            "scope",
            "sts url");
        var configurationGetter = new RegisteredSingletonHelseIdConfigurationGetter(configuration);

        var returnValue = await configurationGetter.GetConfiguration();
        returnValue.Should().BeSameAs(configuration);
    }
    
}
