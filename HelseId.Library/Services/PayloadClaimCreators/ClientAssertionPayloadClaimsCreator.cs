namespace HelseId.Library.Services.PayloadClaimCreators;

// This class generates the (general) claims that are required for the token that is sent
// to HelseID as part of a client assertion (to the PAR endpoint, or to the Token endpoint)
public class ClientAssertionPayloadClaimsCreator : IPayloadClaimsCreatorForClientAssertion
{
    private readonly TimeProvider _timeProvider;
    private readonly IAssertionDetailsCreator _assertionDetailsCreator;

    public ClientAssertionPayloadClaimsCreator(TimeProvider timeProvider, IAssertionDetailsCreator assertionDetailsCreator)
    {
        _timeProvider = timeProvider;
        _assertionDetailsCreator = assertionDetailsCreator;
    }

    public Dictionary<string, object> CreatePayloadClaims(
        HelseIdConfiguration configuration,
        PayloadClaimParameters payloadClaimParameters)
    {
        payloadClaimParameters.ParametersCheck();
        
        var result = AddStandardClaims(configuration);

        if (payloadClaimParameters.UseClientDetailsInClientAssertion && payloadClaimParameters.HasUseOfClientDetails())
        {
            AddAssertionDetailsClaim(payloadClaimParameters, result);
        }
        
        return result;
    }

    private Dictionary<string, object> AddStandardClaims(HelseIdConfiguration configuration)
    {
        // Time values are converted to epoch (UNIX) time format
        var tokenIssuedAtEpochTime = _timeProvider.GetUtcNow().ToUnixTimeSeconds();
        // This class contains JSON objects representing the claims contained in the JWT.
        return new Dictionary<string, object>
        {
            // See https://www.rfc-editor.org/rfc/rfc7523#section-3 for a further description of these claims
            // "iss": the issuer claim; in our case, the value is the client ID
            { JwtRegisteredClaimNames.Iss, configuration.ClientId },
            // "sub" (subject): a unique identifier for the End-User at the Issuer. HelseID expects this to be the client ID.
            { JwtRegisteredClaimNames.Sub, configuration.ClientId },
            // "aud" (audience): the audience for our client assertion is the HelseID server
            { JwtRegisteredClaimNames.Aud, configuration.StsUrl },
            // "exp" (expires at): this describes the end of the token usage period
            { JwtRegisteredClaimNames.Exp, tokenIssuedAtEpochTime + PayloadConfiguration.TokenExpirationTimeInSeconds },
            // "iat" (issued at time): this describes the time when the token was issued
            { JwtRegisteredClaimNames.Iat, tokenIssuedAtEpochTime },
            // "nbf (not before)"
            { JwtRegisteredClaimNames.Nbf, tokenIssuedAtEpochTime },
            // "jti" a unique identifier for the token, which can be used to prevent reuse of the token.
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N") },
        };
    }
    
    private void AddAssertionDetailsClaim(PayloadClaimParameters payloadClaimParameters, Dictionary<string, object> result)
    {
        var assertionDetails = _assertionDetailsCreator.CreateAssertionDetails(payloadClaimParameters);
        result.Add(assertionDetails.Name, assertionDetails.Value);
    }
}
