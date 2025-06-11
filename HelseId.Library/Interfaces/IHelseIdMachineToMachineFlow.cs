using HelseId.Library.Models;
using HelseId.Library.Models.DetailsFromClient;

namespace HelseId.Library;

public interface IHelseIdMachineToMachineFlow
{
    Task<TokenResponse> GetTokenAsync();
    Task<TokenResponse> GetTokenAsync(OrganizationNumbers organizationNumbers);
}
