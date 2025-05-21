using HelseID.Standard.Models.Constants;

namespace HelseID.Standard.Models;


public class HelseIdTokenRequest
{
    public required string GrantType { get; init; }
    
    public required string Address { get; init; }
    
    public required string ClientId { get; init; }
    
    public required string ClientAssertion { get; init; }
    
    public required string DPoPProofToken { get; init; }
    
    public required string Scope { get; init; }
}
