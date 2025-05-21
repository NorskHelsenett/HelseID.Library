using HelseID.Standard.Models;

namespace HelseID.Standard;

public interface IHelseIdTokenRetriever
{
    Task<TokenResponse> GetTokenAsync();
}
