namespace HelseId.Library.Services.Configuration;

public class FileBasedSigningCredentialReference : ISigningCredentialReference
{
    private readonly string _fileName;
    private SigningCredentials? _signingCredentials;
    public FileBasedSigningCredentialReference(string fileName)
    {
        _fileName = fileName;
    }
    public async Task<SigningCredentials> GetSigningCredential()
    {
        if (_signingCredentials == null)
        {
            var jsonWebKey = await File.ReadAllTextAsync(_fileName);
            var securityKey = new JsonWebKey(jsonWebKey);
            _signingCredentials = new SigningCredentials(securityKey, securityKey.Alg);
        }
        return _signingCredentials;
    }

    public async Task UpdateSigningCredential(string jsonWebKey)
    {
        await File.WriteAllTextAsync(_fileName, jsonWebKey);
        _signingCredentials = null;
    }
}
