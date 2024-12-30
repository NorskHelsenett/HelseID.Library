using HelseID.Standard.Interfaces.JwtTokens;

namespace HelseID.Standard.Tests.Mocks;

public class JtiClaimCreatorMock : IJtiClaimCreator
{
    public string CreateJti()
    {
        return "dcf3f87b-51bb-4a82-bf8d-796de229582d";
    }
}
