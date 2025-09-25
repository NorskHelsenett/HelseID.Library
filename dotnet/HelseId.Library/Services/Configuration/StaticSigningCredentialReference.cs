namespace HelseId.Library.Services.Configuration;

public class StaticSigningCredentialReference : ISigningCredentialReference
{
    private readonly SigningCredentials _signingCredential;

    public StaticSigningCredentialReference(SigningCredentials signingCredential)
    {
        _signingCredential = signingCredential;
    }
    
    public Task<SigningCredentials> GetSigningCredential()
    {
        return Task.FromResult(_signingCredential);
    }

    public Task UpdateSigningCredential(string jsonWebKey)
    {
        throw new NotImplementedException("Static references cannot be updated automatically");
    }
}
