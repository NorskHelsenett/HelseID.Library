namespace HelseId.Library.Models.TokenRequests;

// Models a set of parameters for the use of parameters to a client assertion
// This class can be inherited if special use is required for an instance
public abstract class TokenRequestParameters
{
    public PayloadClaimParameters PayloadClaimParameters { get; set; } = new();
    
    public string Scope { get; set; } = string.Empty;
}
