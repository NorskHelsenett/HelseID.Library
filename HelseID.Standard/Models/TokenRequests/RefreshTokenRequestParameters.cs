namespace HelseID.Standard.Models.TokenRequests;

public class RefreshTokenRequestParameters : TokenRequestParameters
{

    public RefreshTokenRequestParameters(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
    
    public RefreshTokenRequestParameters(string refreshToken, string resource) : this(refreshToken)
    {
        HasResourceIndicator = true;
        Resource.Add(resource);
    }
    
    public string RefreshToken { get; }

    public List<string> Resource { get; } = new();
    
    public bool HasResourceIndicator { get; }
}
