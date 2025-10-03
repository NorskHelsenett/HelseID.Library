using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using HelseId.Library.Services.Configuration;

namespace HelseId.Library.Tests.Configuration;

public abstract class ConfigurationTests
{
    protected const string InvalidPrivateKey = """
                                               {
                                                   "kty": "OKP",
                                                   "d": "bVM7WDQzTQ4Rl5cxZwZ9kKgExYg72lKhiJ4GwTnv-kQ",
                                                   "use": "sig",
                                                   "crv": "X25519",
                                                   "x": "wi4qHzZaqP4izZBQZYZ8Chi45hJohzmQUjxaVt-SiXI",
                                                   "alg": "EdDSA"
                                               }
                                               """;

    protected const string GeneralPrivateEcKey = """
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
    
    protected const string GeneralPrivateRsaKey = """
                                                  {
                                                      "p": "-XqVH9xPiD-v7OUAUITJ50rkQnn7stkbaYKUbRcZNIb_0duY3L2r4goQRTIwFdW4gMBAuNYPrX1EJCkgLOPGmUBys6G9h4UIZUTismsdV92NNE_1uKQEDzdsLBfZ7F_YBRXxj63qhEk0Ei5VSsUJ9wXMoy_rfvbHh2t-GvU8TLc",
                                                      "kty": "RSA",
                                                      "q": "zy8Y9Az4830PYAfE1OL948GkSp7X35vjLbAQiX8oFH3cFUTrC_DgJZVVWsSBe8RwYwdWv9cGSNly2c9a_SyqGPv18y5tG55hlJ683WfdR7kJCaiiPz1REK0Ycjuj206pSgFKayLlf9cGsmn6e2D-TvXGCp7fuiTLB8wiP24rv7c",
                                                      "d": "Nw3K1aOeijs8mQB5OEAjRpMHY2XygfitMSCSyQQoCRzvZID4yTJKhdhhhPxjkfJkTHeiK8to6Mht5h34N6mjw1Cxojz_ivbkEbsGWRYw0qDS-SY3UxXdY6cdWnyb13TQpCFqzQeWzxLZNURRrWuU7FAb3hNiZAUpAR_CIUG41V7Wlje0dxuD_SjfD0R-lsc-oU8vQv4xbKxU1yqEm6dw_r7PBUc1mXG3uQwF4QxtcpIo1rzpuEWMXFKPdtzfOBL87cNqU-x_36JX-scqHj82osyxWr4lei_HW7nnwdehXrTpt7z81GZ0d-xWghwswe-cswqgwppuKrRYdzJf1qwOeQ",
                                                      "e": "AQAB",
                                                      "use": "sig",
                                                      "qi": "zw7zJzqMnH-J-xtx5mBVRv1IWqy8T7mDPbVjek-AG6BlfpaPy8Re_Y20okylBpwqPXB10MBBFVDgBxDqNmsi9_DGD1vjGiU-X2xPfupQe9jWeCIImufDOcTb_SSyxaeL-eT3Abyp3Uv9-3jee-J8HyhBQMazLSBCigC1oOwOohU",
                                                      "dp": "Vjud4iZnw0OoTq0VP7-2fmEvpx689qP-yqidH1wVlUd4k5RhMSPKjtZFq3Vek52NKcvKU1mCj8g1zn38pieRVRWm0Bp_BaHhNgfqiBtoSB_HmCZvXAQtdwkENfWEmSTD6XYdSjmmT1VyTzp-ttqGyEDT7DA7KWapr1BjhXPxBN8",
                                                      "alg": "RS384",
                                                      "dq": "YqVmGC1lC2nsPCj2yvYZNKwqcW0YE9vBWyuNdCobB0a111qtH05dKwfjUAhoLmnlqQEBiO30jBMc4CAejpUJmEO_Yz4jrRbIF19NRY4CiffMGdE1Dy8-vdXs4dcLuYvGhvn-WOyQoUHcam6TWNNhwm67EKt8ksCZ-uhzFgh6xBk",
                                                      "n": "yegEZ3dzO9IkGqk_L8gCKLfMSlc9-MSmqYbcWhCTV_HpKCVSk16S-8XFdMgb8J0ZVxIPO_1udCv9V98y79atLrynewm9AzcVLqj8pj4zaYvtdlGMcGNBUXaLjhw57xrnsEOaF-3yg8NDXGwsJeUomPSvb1tu5qMZKPQoAwjwxe0KJXVFmriGom3RsoP2Uh4ce-1iLgVJx-h2_IqZyaCsySWP6doMKV2aRXToyDm6KH592dbMJ49Wqax5Vc2ucnWjfcR7Bj5YtDiaslv8btyNVmXwJVcEzobMrpXqnwHv_7xdkav1GvO6aU1XGIlKLPijie6W9i2JdmBDr4td0BRf0Q"
                                                  }
                                                  """;
    
    protected const string ClientId = "294c7ab7-a22e-4a79-aea3-34edddf5b85d";
    protected const string Scope = "openid profile offline_access";
    protected static string StsUrl { get; } = "https://helseid-sts.test.nhn.no";
    protected HelseIdConfiguration HelseIdConfiguration { get; set; }
    
    protected StaticSigningCredentialReference CredentialReference { get; set; }
    protected StaticSigningCredentialReference CredentialWithEcKey { get; set; }
    protected StaticSigningCredentialReference CredentialWithInvalidKey { get; set; }
    
    protected PayloadClaimParameters PayloadClaimParameters { get; set; }

    protected static StaticSigningCredentialReference GetX509CredentialReference()
    {
        X509Certificate2 certificate = X509CertificateGenerator.GenerateSelfSignedCertificate("HelseID self-signed certificate", X509KeyUsageFlags.NonRepudiation);

        var x509SigningCredentials = new X509SigningCredentials(certificate, "RS384");
        var key = x509SigningCredentials.Key as X509SecurityKey;
        var jsonWebKey = JsonWebKeyConverter.ConvertFromX509SecurityKey(key, representAsRsaKey: true);
        var signingCredental = new SigningCredentials(jsonWebKey, "RS384");
        
        return new StaticSigningCredentialReference(signingCredental);
    }
    
    [SetUp]
    public void SetUpConfiguration()
    {
        HelseIdConfiguration = new HelseIdConfiguration { ClientId = ClientId, Scope = Scope, StsUrl = StsUrl };

        CredentialReference = new StaticSigningCredentialReference(new SigningCredentials(new JsonWebKey(GeneralPrivateRsaKey), "RS384"));
        CredentialWithEcKey = new StaticSigningCredentialReference(new SigningCredentials(new JsonWebKey(GeneralPrivateEcKey), "ES384"));
        CredentialWithInvalidKey = new StaticSigningCredentialReference(new SigningCredentials(new JsonWebKey(InvalidPrivateKey), "EdDSA"));
        
        PayloadClaimParameters = new PayloadClaimParameters
        {
            UseSfmId = true,
        };
    }
}

public static class X509CertificateGenerator
{
    public static X509Certificate2 GenerateSelfSignedCertificate(string subjectName, X509KeyUsageFlags keyUsageFlag)
    {
        var rsa = RSA.Create();
        var certRequest = new CertificateRequest($"CN={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        certRequest.CertificateExtensions.Add(new X509KeyUsageExtension(keyUsageFlag, false));
        return certRequest.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(10));
    }
}
