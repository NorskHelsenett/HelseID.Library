using HelseID.Standard.Interfaces.JwtTokens;

namespace HelseID.Standard.Services.JwtTokens;

public class JtiClaimCreator : IJtiClaimCreator
{
    public string CreateJti()
    {
        return Guid.NewGuid().ToString();
    }
}
