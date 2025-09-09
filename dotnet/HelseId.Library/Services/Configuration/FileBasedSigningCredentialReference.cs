namespace HelseId.Library.Services.Configuration;

public class FileBasedSigningCredentialReference : ISigningCredentialReference
{
    private readonly string _fileName;
    public FileBasedSigningCredentialReference(string fileName)
    {
        _fileName = fileName;
    }
    public async Task<SigningCredentials> GetSigningCredential()
    {
        var jsonWebKey = await File.ReadAllTextAsync(_fileName);
        var securityKey = new JsonWebKey(jsonWebKey);
        var signingCredentials = new SigningCredentials(securityKey, securityKey.Alg);
        return signingCredentials;
    }

    public async Task UpdateSigningCredential(string jsonWebKey)
    {
        await File.WriteAllTextAsync(_fileName, jsonWebKey);
    }
}
