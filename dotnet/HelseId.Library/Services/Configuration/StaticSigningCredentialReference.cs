namespace HelseId.Library.Services.Configuration;

public class StaticSigningCredentialReference : ISigningCredentialReference
{
    private readonly SigningCredentials _signingCredential;

    public StaticSigningCredentialReference(string jsonWebKey)
    {
        var signingKey = new JsonWebKey(jsonWebKey);
        _signingCredential = new SigningCredentials(signingKey, signingKey.Alg);
        
    }

    public StaticSigningCredentialReference(SigningCredentials signingCredential)
    {
        _signingCredential = signingCredential;
    }
    
    public Task<SigningCredentials> GetSigningCredentialReference()
    {
        return Task.FromResult(_signingCredential);
    }
}
