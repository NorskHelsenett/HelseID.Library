namespace HelseId.Standard.Interfaces.JwtTokens;

public interface IDPoPProofCreator
{
    string CreateDPoPProof(
        string url,
        string httpMethod,
        string? dPoPNonce = null,
        string? accessToken = null);
}
