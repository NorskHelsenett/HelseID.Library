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
            try
            {
                var jsonWebKey = await File.ReadAllTextAsync(_jwkFileName);
                var securityKey = new JsonWebKey(jsonWebKey);
                _signingCredentials = new SigningCredentials(securityKey, securityKey.Alg);
            }
            catch (Exception exception)
            {
                throw new HelseIdException("Invalid Json Web Key",
                    $"The file {_jwkFileName} does not contain a valid Json Web Key", 
                    exception);
            }
        }
        return _signingCredentials;
    }
}
