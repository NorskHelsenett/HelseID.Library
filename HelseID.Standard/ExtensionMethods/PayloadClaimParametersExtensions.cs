using HelseId.Standard.Exceptions;
using HelseId.Standard.Models.Payloads;

namespace HelseId.Standard.ExtensionMethods;

public static class PayloadClaimParametersExtensions
{
    public static bool HasUseOfClientDetails(this PayloadClaimParameters claimParameters)
    {
        return (claimParameters.UseOrganizationNumbers || 
                claimParameters.UseSfmId || 
                claimParameters.UseTillitsrammeverk);
    }

    public static void ParametersCheck(this PayloadClaimParameters claimParameters)
    {
        if (claimParameters.UseClientDetailsInClientAssertion && claimParameters.UseClientDetailsInRequestObject)
        {
            throw new PayloadClaimParametersException(
                $"Both {nameof(PayloadClaimParameters.UseClientDetailsInClientAssertion)} and {nameof(PayloadClaimParameters.UseClientDetailsInRequestObject)} cannot be set.");
        }
    }
}
