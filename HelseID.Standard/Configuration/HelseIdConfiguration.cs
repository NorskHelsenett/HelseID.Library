using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace HelseID.Standard.Configuration;

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
        var jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(key);

        return new HelseIdConfiguration(jsonWebKey, algorithm, clientId, scope, stsUrl, resourceIndicators);
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
        var jsonWebKey = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(key);

        return new HelseIdConfiguration(jsonWebKey, algorithm, clientId, scope, stsUrl, resourceIndicators);
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

        return new HelseIdConfiguration(jsonWebKey, algorithm, clientId, scope, stsUrl, resourceIndicators);
    }
    
    public static HelseIdConfiguration ConfigurationForJsonWebKey(
        JsonWebKey jsonWebKey,
        string algorithm,
        string clientId,
        string scope,
        string stsUrl,
        List<string>? resourceIndicators = null)
    {
        return new HelseIdConfiguration(jsonWebKey, algorithm, clientId, scope, stsUrl, resourceIndicators);
    }
    
    private HelseIdConfiguration(
        JsonWebKey jsonWebKey,
        string algorithm,
        string clientId,
        string scope,
        string stsUrl,
        List<string>? resourceIndicators = null)
    {
        SigningCredentials = new SigningCredentials(jsonWebKey, algorithm);
        JsonWebKey = SigningCredentials.Key as JsonWebKey;
        JsonWebKey!.Alg = algorithm;
        ClientId = clientId;
        Scope = scope;
        StsUrl = stsUrl;
        if (resourceIndicators != null)
        {
            ResourceIndicators = resourceIndicators;
        }
    }
    
    public JsonWebKey? JsonWebKey { get; private set; }
    
    public SigningCredentials SigningCredentials { get; private set; }

    public string ClientId { get; private set; }

    public string Scope { get; private set; }

    public string StsUrl { get; private set; }
    
    // Multitenant

    // These are used for clients that are using resource indicators against the PAR and token endpoints:
    public List<string> ResourceIndicators { get; private set; } = new();
}
