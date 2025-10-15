using Microsoft.Extensions.Configuration;

namespace HelseId.Library.Configuration;

/// <summary>
/// This class contains configurations that correspond to clients in HelseID.
/// </summary>
public class HelseIdConfiguration
{
    /// <summary>
    /// The ClientID, can be found i HelseID Selvbetjening
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    /// The default set of scopes that the client will request
    /// </summary>
    public required string Scope { get; init; }

    /// <summary>
    /// The HelseID environment the client is registered in.
    /// Relevant environments are Production: "https://helseid-sts.nhn.no" and Test: "https://helseid-sts.test.nhn.no"
    /// </summary>
    public required string IssuerUri { get; init; } = "https://helseid-sts.nhn.no";

    /// <summary>
    /// Configures Selvbetjening if your client uses automatic key rotation.
    /// </summary>
    public SelvbetjeningConfiguration SelvbetjeningConfiguration { get; set; } = new();

    /// <summary>
    /// Creates a HelseIdConfiguration object from the given configuration section in appsettings.json
    /// </summary>
    /// <param name="configurationSection">The configuration section</param>
    /// <returns>Returns a HelseIdConfiguration object</returns>
    public static HelseIdConfiguration ConfigurationFromAppSettings(IConfigurationSection configurationSection)
    {
        var clientId = configurationSection.GetValue<string>("ClientId")!;
        var stsUrl = configurationSection.GetValue<string>("StsUrl")!;
        var scope = configurationSection.GetValue<string>("Scope")!;

        return new HelseIdConfiguration
        {
            ClientId = clientId,
            Scope = scope,
            IssuerUri = stsUrl,
        };
    }


    /// <summary>
    /// Returns the url of the OIDC Metadata endpoint
    /// </summary>
    /// <returns></returns>
    public string GetMetadataUrl()
    {
        var metadataUrl = IssuerUri;
        if (metadataUrl.EndsWith('/'))
        {
            metadataUrl = metadataUrl.TrimEnd('/');
        }

        metadataUrl += "/.well-known/openid-configuration";

        return metadataUrl;
    }
}
