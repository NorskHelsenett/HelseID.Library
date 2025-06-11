using HelseId.Standard.Models;
using HelseId.Standard.Models.DetailsFromClient;

namespace HelseId.Standard;

public interface IHelseIdMachineToMachineFlow
{
    Task<TokenResponse> GetTokenAsync(OrganizationNumbers organizationNumbers);
}
