using HelseID.Standard.Models;

namespace HelseID.Standard;

public interface IHelseIdMachineToMachineFlow
{
    Task<TokenResponse> GetTokenAsync();
}
