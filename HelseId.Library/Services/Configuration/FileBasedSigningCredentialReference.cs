namespace HelseId.Library.Services.Configuration;

public class FileBasedSigningCredentialReference : ISigningCredentialReference
{
    private readonly string _jwkFileName;
    private SigningCredentials? _signingCredentials;
    public FileBasedSigningCredentialReference(string jwkFileName)
    {
        _jwkFileName = jwkFileName;
    }

    public async Task<SigningCredentials> GetSigningCredential()
    {
        if (_signingCredentials == null)
        {
            var jsonWebKey = await File.ReadAllTextAsync(_jwkFileName);
            var securityKey = new JsonWebKey(jsonWebKey);
            _signingCredentials = new SigningCredentials(securityKey, securityKey.Alg);
        }
        return _signingCredentials;
    }

    public async Task UpdateSigningCredential(string jsonWebKey)
    {
        await File.WriteAllTextAsync(_jwkFileName, jsonWebKey);
        _signingCredentials = null;
    }
}
