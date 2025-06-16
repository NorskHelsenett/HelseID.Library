using Microsoft.Extensions.Configuration;

namespace HelseId.Library.Configuration;

/// <summary>
/// This class contains configurations that correspond to clients in HelseID.
/// </summary>
public class HelseIdConfiguration
{
   public static HelseIdConfiguration ConfigurationForPemRsaCertificate(
        string pem,
        string algorithm,
        string clientId,
        string scope,
        string stsUrl,
        List<string>? resourceIndicators = null)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(pem);
        var key = new RsaSecurityKey(rsa);
        
        return new HelseIdConfiguration(new SigningCredentials(key, algorithm), clientId, scope, stsUrl, resourceIndicators);
    }

    public static HelseIdConfiguration ConfigurationForPemEcCertificate(
        string pem,
        string algorithm,
        string clientId,
        string scope,
        string stsUrl,
        List<string>? resourceIndicators = null)
    {
        var ecDsa = ECDsa.Create();
        ecDsa.ImportFromPem(pem);
        var key = new ECDsaSecurityKey(ecDsa);

        return new HelseIdConfiguration(new SigningCredentials(key, algorithm), clientId, scope, stsUrl, resourceIndicators);
    }
    
    public static HelseIdConfiguration ConfigurationForX509Certificate(
        X509Certificate2 certificate,
        string algorithm,
        string clientId,
        string scope,
        string stsUrl,
        List<string>? resourceIndicators = null)
    {
        var x509SigningCredentials = new X509SigningCredentials(certificate, algorithm);
        var key = x509SigningCredentials.Key as X509SecurityKey;
        var jsonWebKey = JsonWebKeyConverter.ConvertFromX509SecurityKey(key, representAsRsaKey: true);

        return new HelseIdConfiguration(new SigningCredentials(jsonWebKey, algorithm), clientId, scope, stsUrl, resourceIndicators);
    }
    
    public static HelseIdConfiguration ConfigurationForJsonWebKey(
        JsonWebKey jsonWebKey,
        string algorithm,
        string clientId,
        string scope,
        string stsUrl,
        List<string>? resourceIndicators = null)
    {
        return new HelseIdConfiguration(new SigningCredentials(jsonWebKey, algorithm), clientId, scope, stsUrl, resourceIndicators);
    }
     
    public static HelseIdConfiguration ConfigurationForJsonWebKey(
        JsonWebKey jsonWebKey,
        string clientId,
        string scope,
        string stsUrl,
        List<string>? resourceIndicators = null)
    {
        return new HelseIdConfiguration(new SigningCredentials(jsonWebKey, jsonWebKey.Alg), clientId, scope, stsUrl, resourceIndicators);
    }
    
    public static HelseIdConfiguration ConfigurationForJsonWebKey(
        string jsonWebKey,
        string algorithm,
        string clientId,
        string scope,
        string stsUrl,
        List<string>? resourceIndicators = null)
    {
        var signingKey = new JsonWebKey(jsonWebKey);
        return new HelseIdConfiguration(new SigningCredentials(signingKey, algorithm),  clientId, scope, stsUrl, resourceIndicators);
    }
    
    public static HelseIdConfiguration ConfigurationForJsonWebKey(
        string jsonWebKey,
        string clientId,
        string scope,
        string stsUrl,
        List<string>? resourceIndicators = null)
    {
        var signingKey = new JsonWebKey(jsonWebKey);
        return new HelseIdConfiguration(new SigningCredentials(signingKey, signingKey.Alg),  clientId, scope, stsUrl, resourceIndicators);
    }

    public static HelseIdConfiguration ConfigurationFromAppSettings(IConfigurationSection configurationSection)
    {
        var clientId = configurationSection.GetValue<string>("ClientId")!;
        var stsUrl = configurationSection.GetValue<string>("StsUrl")!;
        var scope = configurationSection.GetValue<string>("Scope")!;
        var jwk = configurationSection.GetValue<string>("Jwk")!;
        var jsonWebKey = new JsonWebKey(jwk);
 
        var algorithm = configurationSection.GetValue<string>("Algorithm") ?? jsonWebKey.Alg;

        var signingCredentials = new SigningCredentials(jsonWebKey, algorithm);

        return new HelseIdConfiguration(signingCredentials,
            clientId,
            scope,
            stsUrl);
    }

    public static HelseIdConfiguration ConfigurationFromSigningCredentials(
        SigningCredentials signingCredentials,
        string clientId,
        string scope,
        string stsUrl,
        List<string>? resourceIndicators = null)
    {
        return new HelseIdConfiguration(signingCredentials,
            clientId,
            scope,
            stsUrl,
            resourceIndicators);
    }

    private HelseIdConfiguration(
        SigningCredentials signingCredentials,
        string clientId,
        string scope,
        string stsUrl,
        List<string>? resourceIndicators = null)
    {
        SigningCredentials = signingCredentials;
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
    
    public SigningCredentials SigningCredentials { get; private set; }
    public string ClientId { get; private set; }
    public string Scope { get; private set; }
    public string StsUrl { get; private set; }
    public string MetadataUrl { get; }
    
    // Multitenant

    // These are used for clients that are using resource indicators against the PAR and token endpoints:
    public List<string> ResourceIndicators { get; private set; } = new();
}
