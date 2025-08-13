namespace HelseId.Library.Tests.Mocks;

public class AuthorizationDetailsCreatorMock: IAuthorizationDetailsCreator
{
    public PayloadClaimParameters PayloadClaimParameters { get; set; } = null!;
    
    public PayloadClaim CreateAuthorizationDetails(PayloadClaimParameters payloadClaimParameters)
    {
        PayloadClaimParameters = payloadClaimParameters;
        
        return new PayloadClaim("authorization_details", "some_object_value");
    }
}
