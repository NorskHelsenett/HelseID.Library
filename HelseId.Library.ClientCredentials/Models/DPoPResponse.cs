namespace HelseId.Library.ClientCredentials.Models;

internal sealed class DPoPNonceResponse : TokenResponse
{
    public string? DPoPNonce { get; init; }
}
