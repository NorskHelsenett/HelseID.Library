using HelseId.Library.Services.Configuration;

namespace HelseId.Library.Tests.Services.Configuration;

[TestFixture]
public class StaticSigningCredentialReferenceTest
{
    private const string ValidJwk = """
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

    [Test]
    public async Task GetSigningCredential_returns_registered_signing_credential()
    {
        var signingCredential = new SigningCredentials(new JsonWebKey(ValidJwk), "RS384");
        var signingCredentialReference = new StaticSigningCredentialReference(signingCredential);

        (await signingCredentialReference.GetSigningCredential()).Should().BeSameAs(signingCredential);
    }
}
