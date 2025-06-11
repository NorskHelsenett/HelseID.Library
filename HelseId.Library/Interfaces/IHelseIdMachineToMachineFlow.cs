namespace HelseId.Library.Interfaces;

public interface IHelseIdMachineToMachineFlow
{
    Task<TokenResponse> GetTokenAsync();
    Task<TokenResponse> GetTokenAsync(OrganizationNumbers organizationNumbers);
}
