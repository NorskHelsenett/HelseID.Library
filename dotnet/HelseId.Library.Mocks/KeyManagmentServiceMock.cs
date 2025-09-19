using HelseId.Library.SelfService.Interfaces;
using HelseId.Library.SelfService.Models;

namespace HelseId.Library.Mocks;

public class KeyManagmentServiceMock : IKeyManagementService
{
    public PublicPrivateKeyPair GenerateNewKeyPair()
    {
        return new PublicPrivateKeyPair
        {
            PublicKey = "123",
            PrivateKey = "456"
        };
    }
}
