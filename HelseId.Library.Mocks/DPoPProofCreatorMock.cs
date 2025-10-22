namespace HelseId.Library.Mocks;

public class IdPoPProofCreatorMock : IDPoPProofCreator, IDPoPProofCreatorForApiRequests
{
    public string? Url { get; private set; }
    public string? HttpMethod { get; private set; }
    public string? DPoPNonce { get; private set; }
    public string? AccessToken { get; private set; }

    private readonly string _dPoPProof;
    
    public IdPoPProofCreatorMock(string dPoPProof)
    {
        _dPoPProof = dPoPProof;
    }
    
    public Task<string> CreateDPoPProofForTokenRequest(string url, string httpMethod, string? dPoPNonce = null)
    {
        Url = url;
        HttpMethod = httpMethod;
        DPoPNonce = dPoPNonce;
        return Task.FromResult(_dPoPProof);
    }

    public Task<string> CreateDPoPProofForApiRequest(string url, string httpMethod, string accessToken)
    {
        Url = url;
        HttpMethod = httpMethod;
        AccessToken = accessToken;
        return Task.FromResult(_dPoPProof);
    }

    public Task<string> CreateDPoPProofForApiRequest(string url, string httpMethod, AccessTokenResponse accessTokenResponse)
    {
        Url = url;
        HttpMethod = httpMethod;
        AccessToken = accessTokenResponse.AccessToken;
        return Task.FromResult(_dPoPProof);
    }
}
