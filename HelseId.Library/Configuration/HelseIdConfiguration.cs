using Microsoft.Extensions.Configuration;

namespace HelseId.Library.Configuration;

/// <summary>
/// This class contains configurations that correspond to clients in HelseID.
/// </summary>
public class HelseIdConfiguration
{
    public required string ClientId { get; init; }
    public required string Scope { get; init; }
    public required string IssuerUri { get; init; } = "https://helseid-sts.nhn.no";
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
 
        return new HelseIdConfiguration {
            ClientId = clientId,
            Scope = scope,
            IssuerUri = stsUrl,
        };
    }
    
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
