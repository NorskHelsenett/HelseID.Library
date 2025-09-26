using Microsoft.Extensions.Configuration;

namespace HelseId.Library.Configuration;

/// <summary>
/// This class contains configurations that correspond to clients in HelseID.
/// </summary>
public class HelseIdConfiguration
{
   //  /// <summary>
   //  /// Creates a HelseIdConfiguration object from the given parameters
   //  /// </summary>
   //  /// <param name="pem">The signing key used for client authentication in PEM RSA format</param>
   //  /// <param name="algorithm">The signing algorithm to use</param>
   //  /// <param name="clientId">The client id</param>
   //  /// <param name="scope">The requested scope. Multiple scopes can be requested by space-separating them</param>
   //  /// <param name="stsUrl">The url of the HelseID environment</param>
   //  /// <param name="resourceIndicators"></param>
   //  /// <returns>Returns a HelseIdConfiguration object</returns>
   // public static HelseIdConfiguration ConfigurationForPemRsaCertificate(
   //      string pem,
   //      string algorithm,
   //      string clientId,
   //      string scope,
   //      string stsUrl,
   //      List<string>? resourceIndicators = null)
   //  {
   //      var rsa = RSA.Create();
   //      rsa.ImportFromPem(pem);
   //      var key = new RsaSecurityKey(rsa);
   //      
   //      return new HelseIdConfiguration(new SigningCredentials(key, algorithm), clientId, scope, stsUrl, resourceIndicators);
   //  }
   //
   //  /// <summary>
   //  /// Creates a HelseIdConfiguration object from the given parameters
   //  /// </summary>
   //  /// <param name="pem">The signing key used for client authentication in PEM EC format</param>
   //  /// <param name="algorithm">The signing algorithm to use</param>
   //  /// <param name="clientId">The client id</param>
   //  /// <param name="scope">The requested scope. Multiple scopes can be requested by space-separating them</param>
   //  /// <param name="stsUrl">The url of the HelseID environment</param>
   //  /// <param name="resourceIndicators"></param>
   //  /// <returns>Returns a HelseIdConfiguration object</returns>
   //  public static HelseIdConfiguration ConfigurationForPemEcCertificate(
   //      string pem,
   //      string algorithm,
   //      string clientId,
   //      string scope,
   //      string stsUrl,
   //      List<string>? resourceIndicators = null)
   //  {
   //      var ecDsa = ECDsa.Create();
   //      ecDsa.ImportFromPem(pem);
   //      var key = new ECDsaSecurityKey(ecDsa);
   //
   //      return new HelseIdConfiguration(new SigningCredentials(key, algorithm), clientId, scope, stsUrl, resourceIndicators);
   //  }
   //  
   //  /// <summary>
   //  /// Creates a HelseIdConfiguration object from the given parameters
   //  /// </summary>
   //  /// <param name="certificate">A reference to the certificate used for client authentication</param>
   //  /// <param name="algorithm">The signing algorithm to use</param>
   //  /// <param name="clientId">The client id</param>
   //  /// <param name="scope">The requested scope. Multiple scopes can be requested by space-separating them</param>
   //  /// <param name="stsUrl">The url of the HelseID environment</param>
   //  /// <param name="resourceIndicators"></param>
   //  /// <returns>Returns a HelseIdConfiguration object</returns>
   //  public static HelseIdConfiguration ConfigurationForX509Certificate(
   //      X509Certificate2 certificate,
   //      string algorithm,
   //      string clientId,
   //      string scope,
   //      string stsUrl,
   //      List<string>? resourceIndicators = null)
   //  {
   //      var x509SigningCredentials = new X509SigningCredentials(certificate, algorithm);
   //      var key = x509SigningCredentials.Key as X509SecurityKey;
   //      var jsonWebKey = JsonWebKeyConverter.ConvertFromX509SecurityKey(key, representAsRsaKey: true);
   //
   //      return new HelseIdConfiguration(new SigningCredentials(jsonWebKey, algorithm), clientId, scope, stsUrl, resourceIndicators);
   //  }
   //  
   //  
   //  
   //  
   //  /// <summary>
   //  /// Creates a HelseIdConfiguration object from the given parameters
   //  /// </summary>
   //  /// <param name="jsonWebKey">The JsonWebKey used for client authentication</param>
   //  /// <param name="algorithm">The signing algorithm to use</param>
   //  /// <param name="clientId">The client id</param>
   //  /// <param name="scope">The requested scope. Multiple scopes can be requested by space-separating them</param>
   //  /// <param name="stsUrl">The url of the HelseID environment</param>
   //  /// <param name="resourceIndicators"></param>
   //  /// <returns>Returns a HelseIdConfiguration object</returns>
   //  public static HelseIdConfiguration ConfigurationForJsonWebKey(
   //      JsonWebKey jsonWebKey,
   //      string algorithm,
   //      string clientId,
   //      string scope,
   //      string stsUrl,
   //      List<string>? resourceIndicators = null)
   //  {
   //      return new HelseIdConfiguration(new SigningCredentials(jsonWebKey, algorithm), clientId, scope, stsUrl, resourceIndicators);
   //  }
   //   
   //  /// <summary>
   //  /// Creates a HelseIdConfiguration object from the given parameters
   //  /// </summary>
   //  /// <param name="jsonWebKey">The JsonWebKey used for client authentication. The key must include the alg-parameter.</param>
   //  /// <param name="clientId">The client id</param>
   //  /// <param name="scope">The requested scope. Multiple scopes can be requested by space-separating them</param>
   //  /// <param name="stsUrl">The url of the HelseID environment</param>
   //  /// <param name="resourceIndicators"></param>
   //  /// <returns>Returns a HelseIdConfiguration object</returns>
   //  public static HelseIdConfiguration ConfigurationForJsonWebKey(
   //      JsonWebKey jsonWebKey,
   //      string clientId,
   //      string scope,
   //      string stsUrl,
   //      List<string>? resourceIndicators = null)
   //  {
   //      return new HelseIdConfiguration(new SigningCredentials(jsonWebKey, jsonWebKey.Alg), clientId, scope, stsUrl, resourceIndicators);
   //  }
   //  
   //  /// <summary>
   //  /// Creates a HelseIdConfiguration object from the given parameters
   //  /// </summary>
   //  /// <param name="jsonWebKey">The JsonWebKey used for client authentication</param>
   //  /// <param name="algorithm">The signing algorithm to use</param>
   //  /// <param name="clientId">The client id</param>
   //  /// <param name="scope">The requested scope. Multiple scopes can be requested by space-separating them</param>
   //  /// <param name="stsUrl">The url of the HelseID environment</param>
   //  /// <param name="resourceIndicators"></param>
   //  /// <returns>Returns a HelseIdConfiguration object</returns>
   //  public static HelseIdConfiguration ConfigurationForJsonWebKey(
   //      string jsonWebKey,
   //      string algorithm,
   //      string clientId,
   //      string scope,
   //      string stsUrl,
   //      List<string>? resourceIndicators = null)
   //  {
   //      var signingKey = new JsonWebKey(jsonWebKey);
   //      return new HelseIdConfiguration(new SigningCredentials(signingKey, algorithm),  clientId, scope, stsUrl, resourceIndicators);
   //  }
   //  
   //  /// <summary>
   //  /// Creates a HelseIdConfiguration object from the given parameters
   //  /// </summary>
   //  /// <param name="jsonWebKey">The JsonWebKey used for client authentication. The key must include the alg-parameter.</param>
   //  /// <param name="clientId">The client id</param>
   //  /// <param name="scope">The requested scope. Multiple scopes can be requested by space-separating them</param>
   //  /// <param name="stsUrl">The url of the HelseID environment</param>
   //  /// <param name="resourceIndicators"></param>
   //  /// <returns>Returns a HelseIdConfiguration object</returns>
   //  public static HelseIdConfiguration ConfigurationForJsonWebKey(
   //      string jsonWebKey,
   //      string clientId,
   //      string scope,
   //      string stsUrl,
   //      List<string>? resourceIndicators = null)
   //  {
   //      var signingKey = new JsonWebKey(jsonWebKey);
   //      return new HelseIdConfiguration(new SigningCredentials(signingKey, signingKey.Alg),  clientId, scope, stsUrl, resourceIndicators);
   //  }
   //
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

       return new HelseIdConfiguration(clientId, scope, stsUrl);
   }
   
   /// <summary>
   /// Creates a HelseIdConfiguration object from the HelseID section of the given configuration 
   /// </summary>
   /// <returns>Returns a HelseIdConfiguration object</returns>
   public static HelseIdConfiguration ConfigurationFromAppSettings(IConfiguration configuration)
   {
       var configurationSection = configuration.GetSection("HelseID");
       return ConfigurationFromAppSettings(configurationSection); 
   }
   //  
   //  /// <summary>
   //  /// Creates a HelseIdConfiguration object from the given parameters
   //  /// </summary>
   //  /// <param name="signingCredentials">A generic signing credential used for client authentication</param>
   //  /// <param name="clientId">The client id</param>
   //  /// <param name="scope">The requested scope. Multiple scopes can be requested by space-separating them</param>
   //  /// <param name="stsUrl">The url of the HelseID environment</param>
   //  /// <param name="resourceIndicators"></param>
   //  /// <returns>Returns a HelseIdConfiguration object</returns>
   //  public static HelseIdConfiguration CreateConfiguration(
   //      string clientId,
   //      string scope,
   //      string stsUrl,
   //      List<string>? resourceIndicators = null)
   //  {
   //      return new HelseIdConfiguration(clientId,
   //          scope,
   //          stsUrl,
   //          resourceIndicators);
   //  }

    public HelseIdConfiguration(
        string clientId,
        string scope,
        string stsUrl,
        List<string>? resourceIndicators = null)
    {
        ClientId = clientId;
        Scope = scope;
        StsUrl = stsUrl;
 
        MetadataUrl = MetadataUrlFromStsUrl(stsUrl);

        if (resourceIndicators != null)
        {
            ResourceIndicators = resourceIndicators;
        }
    }

    private static string MetadataUrlFromStsUrl(string stsUrl)
    {
        var metadataUrl = stsUrl;
        if (metadataUrl.EndsWith('/'))
        {
            metadataUrl = metadataUrl.TrimEnd('/');
        }
        metadataUrl += "/.well-known/openid-configuration";

        return metadataUrl;
    }
    
    public string ClientId { get; private set; }
    public string Scope { get; private set; }
    public string StsUrl { get; private set; }
    public string MetadataUrl { get; }
    
    // Multitenant

    // These are used for clients that are using resource indicators against the PAR and token endpoints:
    public List<string> ResourceIndicators { get; private set; } = new();
}
