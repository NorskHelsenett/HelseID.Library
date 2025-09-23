using HelseId.Library.Interfaces.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Library.Mocks;

public class SigningCredentialsReferenceMock : ISigningCredentialReference
{
    
    public string JsonWebKey { get; set; } = string.Empty;
    
    public Task<SigningCredentials> GetSigningCredential()
    {
        throw new NotImplementedException();
    }

    public Task UpdateSigningCredential(string jsonWebKey)
    {
        JsonWebKey = jsonWebKey;
        return Task.CompletedTask;
    }
}
