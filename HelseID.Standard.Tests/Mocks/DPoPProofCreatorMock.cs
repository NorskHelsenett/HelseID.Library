using HelseId.Standard.Interfaces.JwtTokens;

namespace HelseId.Standard.Tests.Mocks;

public class DPoPProofCreatorMock : IDPoPProofCreator
{
    public string Url { get; set; } = null!;
    public string HttpMethod { get; set; } = null!;
    public string? DPoPNonce { get; set; } = null!;
    public string? AccessToken { get; set; } = null!;

    public const string DPoPProof = "eyJhbGciOiJSUzM4NCIsInR5cCI6ImRwb3Arand0IiwiandrIjp7Imt0eSI6IlJTQSIsIm4iOiJ5ZWdFWjNkek85SWtHcWtfTDhnQ0tMZk1TbGM5LU1TbXFZYmNXaENUVl9IcEtDVlNrMTZTLThYRmRNZ2I4SjBaVnhJUE9fMXVkQ3Y5Vjk4eTc5YXRMcnluZXdtOUF6Y1ZMcWo4cGo0emFZdnRkbEdNY0dOQlVYYUxqaHc1N3hybnNFT2FGLTN5ZzhORFhHd3NKZVVvbVBTdmIxdHU1cU1aS1BRb0F3and4ZTBLSlhWRm1yaUdvbTNSc29QMlVoNGNlLTFpTGdWSngtaDJfSXFaeWFDc3lTV1A2ZG9NS1YyYVJYVG95RG02S0g1OTJkYk1KNDlXcWF4NVZjMnVjbldqZmNSN0JqNVl0RGlhc2x2OGJ0eU5WbVh3SlZjRXpvYk1ycFhxbndIdl83eGRrYXYxR3ZPNmFVMVhHSWxLTFBpamllNlc5aTJKZG1CRHI0dGQwQlJmMFEiLCJlIjoiQVFBQiIsImFsZyI6IlJTMzg0In19.eyJpYXQiOjE3MzU5OTQyMjAsImp0aSI6ImRjZjNmODdiLTUxYmItNGE4Mi1iZjhkLTc5NmRlMjI5NTgyZCIsImh0bSI6IlBPU1QiLCJodHUiOiJodHRwczovL2hlbHNlaWQtc3RzLm5obi5uby9jb25uZWN0L3Rva2VuIn0.cesH2QnBFlCPITDHXYwhnlNvhjgOWCo_Yc-BvhH_F89WSWJytbYy2RsWka5_hF5omIHunRkXb8lnJ0vWHyDmiT7jrn97jUs4ymzwmLL6YV-hRQIHQ94mwLC4qWWt8V3zB6sdBoDg2885Xe9h7MElaquFcVxKjeXu9cHz6_kECDPpmy-sQ0i7LrzKLOGRn5SBfEfrADMFfMf41ftW9RIIUw5a1AZQEucK8wa5qyxrk0EmwZo6uu6MEOmNaLFF7L4Q7zUeYyhGH1qjyyTmaqDmn76sp_HBoTvpUeItA__Mxy68R5HBHILxB3-btDW-h7D1uuBO5vuQ_i5u98h81SNyqQ";
    
    public string CreateDPoPProof(string url, string httpMethod, string? dPoPNonce = null, string? accessToken = null)
    {
        Url = url;
        HttpMethod = httpMethod;
        DPoPNonce = dPoPNonce;
        AccessToken = accessToken;
        return DPoPProof;
    }
}
