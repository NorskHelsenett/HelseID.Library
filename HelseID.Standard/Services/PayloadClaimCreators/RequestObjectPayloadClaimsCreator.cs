using HelseId.Standard.Configuration;
using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Models.Configuration;
using HelseId.Standard.Models.Constants;
using HelseId.Standard.Models.Payloads;
using HelseId.Standard.ExtensionMethods;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Standard.Services.PayloadClaimCreators;

// This class generates the claims that are required for the token that is sent
// to HelseID as a request object (to the PAR endpoint)
public class RequestObjectPayloadClaimsCreator : IPayloadClaimsCreatorForRequestObjects
{
    private readonly TimeProvider _timeProvider;
    private readonly IAuthorizationDetailsCreator _authorizationDetailsCreator;

    public RequestObjectPayloadClaimsCreator(TimeProvider timeProvider, IAuthorizationDetailsCreator authorizationDetailsCreator)
    {
        _timeProvider = timeProvider;
        _authorizationDetailsCreator = authorizationDetailsCreator;
    }

    public Dictionary<string, object> CreatePayloadClaims(
        HelseIdConfiguration configuration,
        PayloadClaimParameters payloadClaimParameters)
    {
        payloadClaimParameters.ParametersCheck();
        
        var result = new Dictionary<string, object>();
        
        if (payloadClaimParameters.UseRequestObjects)
        {
            AddStandardClaims(result, configuration);

            if (payloadClaimParameters.UseClientDetailsInRequestObject && payloadClaimParameters.HasUseOfClientDetails())
            {
                AddAuthorizationDetailsClaim(payloadClaimParameters, result);
            }
        }

        return result;
    }

    private void AddStandardClaims(Dictionary<string, object> result, HelseIdConfiguration configuration)
    {
        // Time values are converted to epoch (UNIX) time format
        var tokenIssuedAtEpochTime = EpochTime.GetIntDate(_timeProvider.GetUtcNow().DateTime);
        // This class contains JSON objects representing the claims contained in the JWT.

        // See https://utviklerportal.nhn.no/informasjonstjenester/helseid/bruksmoenstre-og-eksempelkode/bruk-av-helseid/docs/tekniske-mekanismer/organisasjonsnumre_no_nbmd/,
        // and also https://openid.net/specs/openid-connect-core-1_0.html#RequestObject for a further description of these claims
        // "iss": the issuer claim; in our case, the value is the client ID
        result.Add(JwtRegisteredClaimNames.Iss, configuration.ClientId);
        // "aud" (audience): the audience for our client assertion is the HelseID server
        result.Add(JwtRegisteredClaimNames.Aud, configuration.StsUrl);
        // "exp" (expires at): this describes the end of the token usage period
        result.Add(JwtRegisteredClaimNames.Exp, tokenIssuedAtEpochTime + PayloadConfiguration.TokenExpirationTimeInSeconds);
        // "nbf" (not before): this describes the start of the token usage period
        result.Add(JwtRegisteredClaimNames.Nbf, tokenIssuedAtEpochTime);
        // "client_id": the value of the current client ID
        result.Add(HelseIdConstants.ClientIdClaimName, configuration.ClientId);
        // "jti" a unique identifier for the token, which can be used to prevent reuse of the token.
        result.Add(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"));
    }
    
    private void AddAuthorizationDetailsClaim(PayloadClaimParameters payloadClaimParameters, Dictionary<string, object> result)
    {
        var authorizationDetails = _authorizationDetailsCreator.CreateAuthorizationDetails(payloadClaimParameters);
        result.Add(authorizationDetails.Name, authorizationDetails.Value);
    }
}
