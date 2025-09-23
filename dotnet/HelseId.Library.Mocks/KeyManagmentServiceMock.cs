using HelseId.Library.SelfService.Interfaces;
using HelseId.Library.SelfService.Models;

namespace HelseId.Library.Mocks;

public class KeyManagmentServiceMock : IKeyManagementService
{
    public int GenerateSet { get; set; }
    
    public PublicPrivateKeyPair GenerateNewKeyPair()
    {
        GenerateSet += 1;
        
        return new PublicPrivateKeyPair
        {
            PublicKey = "123",
            PrivateKey = "456"
        };
    }
}
