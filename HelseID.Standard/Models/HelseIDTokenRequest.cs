using HelseID.Standard.Models.Constants;

namespace HelseID.Standard.Models;


public class HelseIdTokenRequest
{
    public string GrantType { get; set; } = GrantTypes.GrantType;
    
    public string? Address { get; set; }
    
    public string? ClientId { get; set; }
    
    public string? ClientAssertion { get; set; }
    
    public string? DPoPProofToken { get; set; }
    
    public string? Scope { get; set; }
}
