using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Standard.Tests.Configuration;

[TestFixture]
public class HelseIdConfigurationTests : ConfigurationTests
{
    [Test]
    public void Create_configuration_with_jwk_key()
    {
        JsonWebKey jsonWebKey = new JsonWebKey(GeneralPrivateEcKey);
        
        var configuration = HelseIdConfiguration.ConfigurationForJsonWebKey(
            jsonWebKey,
            ClientId,
            Scope,
            StsUrl);

        configuration.SigningCredentials.Key.Should().Be(jsonWebKey);
        configuration.SigningCredentials.Algorithm.Should().Be(SecurityAlgorithms.EcdsaSha384);
    }

    [Test]
    public void Create_configuration_with_jwk_key_and_other_parameters()
    {
        JsonWebKey jsonWebKey = new JsonWebKey(GeneralPrivateEcKey);
        var resourceIndicators = new List<string>()
        {
            "foo",
            "bar"
        };
        
        var configuration = HelseIdConfiguration.ConfigurationForJsonWebKey(
            jsonWebKey,
            ClientId,
            Scope,
            StsUrl,
            resourceIndicators);

        configuration.ClientId.Should().Be(ClientId);
        configuration.Scope.Should().Be(Scope);
        configuration.StsUrl.Should().Be(StsUrl);
        configuration.ResourceIndicators.Should().BeEquivalentTo(resourceIndicators);
    }
    
    [Test]
    public void Create_configuration_does_not_set_resource_indicators()
    {
        JsonWebKey jsonWebKey = new JsonWebKey(GeneralPrivateEcKey);
        
        var configuration = HelseIdConfiguration.ConfigurationForJsonWebKey(
            jsonWebKey,
            ClientId,
            Scope,
            StsUrl);

        configuration.ResourceIndicators.Should().BeEmpty();
    }
    
    [Test]
    public void Create_configuration_with_X509SigningCredentials()
    {
        X509Certificate2 certificate = X509CertificateGenerator.GenerateSelfSignedCertificate("HelseID self-signed certificate", X509KeyUsageFlags.NonRepudiation);
        
        var configuration = HelseIdConfiguration.ConfigurationForX509Certificate(
            certificate,
            "RS384",
            ClientId,
            Scope,
            StsUrl);

        configuration.SigningCredentials.Algorithm.Should().Be(SecurityAlgorithms.RsaSha384);
        configuration.SigningCredentials.Key.Should().NotBeNull();
    }
    
    [Test]
    public void Create_configuration_with_pem_rsa_certificate()
    {
        var configuration = HelseIdConfiguration.ConfigurationForPemRsaCertificate(
            PemRsa,
            SecurityAlgorithms.RsaSha384,
            ClientId,
            Scope,
            StsUrl);

        configuration.SigningCredentials.Algorithm.Should().Be(SecurityAlgorithms.RsaSha384);
        configuration.SigningCredentials.Key.Should().NotBeNull();
    }
    
    [Test]
    public void Create_configuration_with_pem_ec_certificate()
    {
        var configuration = HelseIdConfiguration.ConfigurationForPemEcCertificate(
            PemEc,
            SecurityAlgorithms.EcdsaSha384,
            ClientId,
            Scope,
            StsUrl);

        configuration.SigningCredentials.Algorithm.Should().Be(SecurityAlgorithms.EcdsaSha384);
        configuration.SigningCredentials.Key.Should().NotBeNull();
    }

    [Test]
    public void Create_configuration_from_configuration_section_without_algorithm_specified()
    {
        var configValues = new Dictionary<string, string?>
        {
            { "HelseId:ClientId", "client id"},
            { "HelseId:StsUrl", "sts url"},
            { "HelseId:Scope", "scope"},
            { "HelseId:Jwk", GeneralPrivateEcKey},
        };
        
        var configRoot = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();
        var configSection = configRoot.GetSection("HelseId");

        var configuration = HelseIdConfiguration.ConfigurationFromAppSettings(configSection);
        
        configuration.ClientId.Should().Be("client id");
        configuration.StsUrl.Should().Be("sts url");
        configuration.Scope.Should().Be("scope");
        configuration.SigningCredentials.Algorithm.Should().Be("ES384");
        configuration.SigningCredentials.Kid.Should().Be("kidvalue"); // kid from jwk
    }
    
    [Test]
    public void Create_configuration_from_configuration_section_with_algorithm_specified()
    {
        var configValues = new Dictionary<string, string?>
        {
            { "HelseId:ClientId", "client id"},
            { "HelseId:StsUrl", "sts url"},
            { "HelseId:Scope", "scope"},
            { "HelseId:Algorithm", "ES256"},
            { "HelseId:Jwk", GeneralPrivateEcKey},
        };
        
        var configRoot = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();
        var configSection = configRoot.GetSection("HelseId");

        var configuration = HelseIdConfiguration.ConfigurationFromAppSettings(configSection);
        
        configuration.ClientId.Should().Be("client id");
        configuration.StsUrl.Should().Be("sts url");
        configuration.Scope.Should().Be("scope");
        configuration.SigningCredentials.Algorithm.Should().Be("ES256");
        configuration.SigningCredentials.Kid.Should().Be("kidvalue"); // kid from jwk
    }

    [Test]
    public void Create_configuration_from_signing_credentials()
    {
        var jsonWebKey = new JsonWebKey(GeneralPrivateEcKey);
        var signingCredentials = new SigningCredentials(jsonWebKey, jsonWebKey.Alg);
        
        var resourceIndicators = new List<string>()
        {
            "foo",
            "bar"
        };

        var configuration = HelseIdConfiguration.ConfigurationFromSigningCredentials(signingCredentials,
            ClientId,
            Scope,
            StsUrl,
            resourceIndicators);

        configuration.SigningCredentials.Should().BeSameAs(signingCredentials);
        configuration.ClientId.Should().Be(ClientId);
        configuration.Scope.Should().Be(Scope);
        configuration.StsUrl.Should().Be(StsUrl);
        configuration.ResourceIndicators.Should().BeEquivalentTo(resourceIndicators);
    }
}
