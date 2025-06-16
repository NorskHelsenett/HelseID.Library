namespace HelseId.Library.Interfaces.JwtTokens;

public interface IDPoPProofCreator
{
    Task<string> CreateDPoPProof(
        string url,
        string httpMethod,
        string? dPoPNonce = null,
        string? accessToken = null);
}
