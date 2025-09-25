using Microsoft.Extensions.Configuration;

namespace HelseId.Library.Tests.Configuration;

[TestFixture]
public class HelseIdConfigurationTests : ConfigurationTests
{
    [Test]
    public void Create_configuration_from_configuration_section_without_algorithm_specified()
    {
        var configValues = new Dictionary<string, string?>
        {
            { "HelseId:ClientId", "client id"},
            { "HelseId:StsUrl", "sts url"},
            { "HelseId:Scope", "scope"},
        };
        
        var configRoot = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();
        var configSection = configRoot.GetSection("HelseId");

        var configuration = HelseIdConfiguration.ConfigurationFromAppSettings(configSection);
        
        configuration.ClientId.Should().Be("client id");
        configuration.StsUrl.Should().Be("sts url");
        configuration.Scope.Should().Be("scope");
    }
    
    [Test]
    public void Create_configuration_from_configuration_section_with_algorithm_specified()
    {
        var configValues = new Dictionary<string, string?>
        {
            { "HelseId:ClientId", "client id"},
            { "HelseId:StsUrl", "sts url"},
            { "HelseId:Scope", "scope"},
        };
        
        var configRoot = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();
        var configSection = configRoot.GetSection("HelseId");

        var configuration = HelseIdConfiguration.ConfigurationFromAppSettings(configSection);
        
        configuration.ClientId.Should().Be("client id");
        configuration.StsUrl.Should().Be("sts url");
        configuration.Scope.Should().Be("scope");
    }
}
