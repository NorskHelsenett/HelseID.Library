using HelseID.Standard.Models;
using HelseID.Standard.Models.DetailsFromClient;

namespace HelseID.Standard;

public interface IHelseIdMachineToMachineFlow
{
    Task<TokenResponse> GetTokenAsync(OrganizationNumbers organizationNumbers);
}
