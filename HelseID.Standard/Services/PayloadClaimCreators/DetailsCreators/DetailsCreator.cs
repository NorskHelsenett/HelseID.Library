using HelseID.Standard.Exceptions;
using HelseID.Standard.Interfaces.PayloadClaimCreators;
using HelseID.Standard.Models.Payloads;

namespace HelseID.Standard.Services.PayloadClaimCreators.DetailsCreators;

public abstract class DetailsCreator
{
    private readonly List<IStructuredClaimsCreator> _structuredClaimsCreators;

    protected DetailsCreator(List<IStructuredClaimsCreator> structuredClaimsCreators)
    {
        _structuredClaimsCreators = structuredClaimsCreators;
    }

    protected PayloadClaim CreateDetails(PayloadClaimParameters payloadClaimParameters)
    {
        var payloadValue = new List<object>();
        foreach (IStructuredClaimsCreator structuredClaimsCreator in _structuredClaimsCreators)
        {
            var value = structuredClaimsCreator.CreateStructuredClaims(payloadClaimParameters, out var claimType);
            
            if (value == false)
            {
                throw new MissingValueFromStructuredClaimsCreatorException();
            }
            
            payloadValue.Add(claimType);
        }

        if (payloadValue.Count == 1)
        {
            return new PayloadClaim(DetailsName, payloadValue.First());    
        }
        
        return new PayloadClaim(DetailsName, payloadValue);
    }
    
    protected string DetailsName { get; init; } = null!;
}
