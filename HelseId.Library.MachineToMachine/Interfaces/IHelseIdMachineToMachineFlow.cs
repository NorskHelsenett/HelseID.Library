namespace HelseId.Library.MachineToMachine.Interfaces;

public interface IHelseIdMachineToMachineFlow
{
    Task<TokenResponse> GetTokenResponseAsync();
    Task<TokenResponse> GetTokenResponseAsync(OrganizationNumbers organizationNumbers);
    Task<string> GetAccessTokenAsync();
    Task<string> GetAccessTokenAsync(OrganizationNumbers organizationNumbers);

}
