using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace HelseId.Library.Tests.Configuration;

public static class X509CertificateGenerator
{
    public static X509Certificate2 GenerateSelfSignedCertificate(
        string subjectName,
        X509KeyUsageFlags keyUsageFlag,
        bool onlyPublicKey = false)
    {
        var rsa = RSA.Create();
        var certRequest = new CertificateRequest($"CN={subjectName}",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        certRequest.CertificateExtensions.Add(new X509KeyUsageExtension(keyUsageFlag, false));
        var certificate = certRequest.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(10));

        if (onlyPublicKey)
        {
            return X509CertificateLoader.LoadCertificate(certificate.RawData);
        }

        return certificate;
    }
}
