namespace HelseId.Library.Services.Configuration;

public class StaticSigningCredentialReference : ISigningCredentialReference
{
    private readonly SigningCredentials _signingCredential;

    public StaticSigningCredentialReference(SigningCredentials signingCredential)
    {
        _signingCredential = signingCredential;
    }
    
    public Task<SigningCredentials> GetSigningCredentialReference()
    {
        return Task.FromResult(_signingCredential);
    }
}
