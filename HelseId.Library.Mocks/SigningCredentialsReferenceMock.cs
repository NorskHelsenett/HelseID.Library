using HelseId.Library.Interfaces.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Library.Mocks;

public class SigningCredentialsReferenceMock : ISigningCredentialReference
{
    public string Jwk { get; set; } = string.Empty;

    public Task<SigningCredentials> GetSigningCredential()
    {
        var jsonWebKey = new JsonWebKey(Jwk);
        return Task.FromResult(new SigningCredentials(jsonWebKey, jsonWebKey.Kid));
    }
}
