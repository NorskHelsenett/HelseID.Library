using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Standard.Tests.Configuration;

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
                                                     "alg": "ES384"
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
    
    protected const string PemEc = """
                                   -----BEGIN PRIVATE KEY-----
                                   ME4CAQAwEAYHKoZIzj0CAQYFK4EEACIENzA1AgEBBDC56yGUkooKZDcOrlZ7llny
                                   isW3rDX+VEKinSyhufyFWnXzFyHHitejl1wmSWCaPfQ=
                                   -----END PRIVATE KEY-----
                                   """;
    
    protected const string PemRsa = """
                                    -----BEGIN PRIVATE KEY-----
                                    MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDCG1+ae4voyaSA
                                    JmDJHUSwY/91VY0Q9x9n76Y/CBT1szU3nldtJNd2ITpue55ZfbfABzo14ckyrPaE
                                    /8Ls6/XD2FSlcD9T8BaX4BuGLwWW0TTOokuF+52ProiWIFSLr0xtDjXImH74MjvH
                                    BgvSoCyT5c4gYmKB1rcBtnDQFF3J6zjyokjtuHfJqqW9I9CqZVyvgZaTqNhax5Qp
                                    W20sIPgOtyPtPFWxXfMh6aQYEhE5J0idqnEAUH9+ixspJ3pPBfJu26I4oe501zx5
                                    F2oBwe/B8aRoYdIklfxstIZumxNRg2m40OZAP53MhHqpd6GulybaBZ/y3iHH2JXd
                                    x1alCuYLAgMBAAECggEAay5qUAdAbj5J02ldsC+1KiFNgiDHUELk2KOADPYNfM6D
                                    6D5K0D2zq8sVVMR137yHpkVXY0FVs+HGDhKcM+7nOklOFDwy+8pq71oUWLfGQk0q
                                    956uTUFu81PQn/UiOi+F6AIcuLNcCPjUAUXZaWiAtPoEI+Wvtzo0u4FeMcfNMhz2
                                    GM/enNSzoXd2oTPKiCoAV91vMcvzruDxuVFnqa24w9wm9+JxqQvk/e0YN00b42Il
                                    LOCMm27Pv4XaxGeNNCujr5+E5M0JFC0z50tAXbenGftfTP4hfbupfOJRcuXJnoiG
                                    BUT2N214b0MbgzwJu0XnBlp/kmbw9m67rxmx1q17wQKBgQDtsfZ2kZjfOBr1WuVc
                                    QQJAMnCzkMzDsz7SjTHPkOwnHbmqJVZBoX1puUQBxE1H5Lo40TFrWsMzydKUZqA2
                                    HUQZ2w9MOdaoIwG1TOF2YfCN5SbdEyOhZm+a1QigFGvp+1xPdrUSAjwyU5rbS1HU
                                    Pvcp6voWq1jiwigmFq+Xn00NiQKBgQDRDhdTKU606BIUUqnmKiPr1VbJREJkF2ox
                                    Dfpab7uar3oTAc/iAkFhwJQL9LX2+AXjWgAgz8ZRvrK82N3JRoye+HR/2AOSX18/
                                    Uu00DIiY8cyNYPnWHAe2kEJWM09cA3umLC+csXANPTz3A6h4PVgnobMRNdE6LHxw
                                    z8xisBBl8wKBgQCEk8COfcMwrhCLTXmkrTXeLMQjw94SQcYGlm50AbdWUgQe3Nhx
                                    nutertE50w3vzan4yuWvpV4/YpCr6VnvFP/JFCBCOfh6q5vmTc1rxFDyNCArKuOy
                                    wHn7eFtpPjq6tVLVl7aDJvAAehVjv20S31Po84EuZ8AaKoVOSVUDUv0dUQKBgGE4
                                    mmRUW4QKQQ4ue1DM3DO63UdwdSezM/FoPLt+JtMDNFROjWzf+6QIOVFuFomqQqK+
                                    ojygI+y0MM9eU5ZdNtxfU155CQAzVkpOuH1yLrxiBuzg4o9OLdAbOp538joqbICV
                                    H/dRFXkyt31wuBJjBPI5YttoGctiEOUt3jEvDBE9AoGAVPgJhNFg5y5DsDi1dtxl
                                    yTzvo6b6gsVegyUF7Kx/78HvwKsNRYyDgGe2uJUGuuSoEw4jaiXKh33r9EB4EQJk
                                    iaj3/q63Ij9UMhVff3Bcz5TKwArn7BwQuwSSLvJMOZd2olb2VknUkqRpWFFgaDqr
                                    GdnbMzwV9MTjV06Hi5uKB2Q=
                                    -----END PRIVATE KEY-----
                                    """;
    
    protected const string ClientId = "294c7ab7-a22e-4a79-aea3-34edddf5b85d";
    protected const string Scope = "openid profile offline_access";
    protected static string StsUrl { get; } = "https://helseid-sts.test.nhn.no";
    protected HelseIdConfiguration HelseIdConfiguration { get; set; }
    protected HelseIdConfiguration HelseIdConfigurationWithEcKey { get; set; }
    
    protected HelseIdConfiguration HelseIdConfigurationWithInvalidKey { get; set; }

    // protected HelseIdConfiguration HelseIdConfigurationWithX509 { get; set; }

    protected PayloadClaimParameters PayloadClaimParameters { get; set; }

    protected static HelseIdConfiguration SetHelseIdConfigurationWithX509()
    {
        X509Certificate2 certificate = X509CertificateGenerator.GenerateSelfSignedCertificate("HelseID self-signed certificate", X509KeyUsageFlags.NonRepudiation);

        return HelseIdConfiguration.ConfigurationForX509Certificate(certificate,"RS384", ClientId, Scope, StsUrl);
    }
    
    [SetUp]
    public void SetUpConfiguration()
    {
        HelseIdConfiguration = HelseIdConfiguration.ConfigurationForJsonWebKey(new JsonWebKey(GeneralPrivateRsaKey), ClientId, Scope, StsUrl);
        HelseIdConfigurationWithEcKey = HelseIdConfiguration.ConfigurationForJsonWebKey(new JsonWebKey(GeneralPrivateEcKey), ClientId, Scope, StsUrl);
        HelseIdConfigurationWithInvalidKey = HelseIdConfiguration.ConfigurationForJsonWebKey(new JsonWebKey(InvalidPrivateKey), ClientId, Scope, StsUrl);
        
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
