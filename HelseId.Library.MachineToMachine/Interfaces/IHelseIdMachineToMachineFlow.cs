namespace HelseId.Library.MachineToMachine.Interfaces;

public interface IHelseIdMachineToMachineFlow
{
    Task<TokenResponse> GetTokenAsync();
    Task<TokenResponse> GetTokenAsync(OrganizationNumbers organizationNumbers);
}
