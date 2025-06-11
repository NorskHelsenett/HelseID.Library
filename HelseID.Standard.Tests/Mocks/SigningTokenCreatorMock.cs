using HelseId.Standard.Interfaces.JwtTokens;
using HelseId.Standard.Interfaces.PayloadClaimCreators;
using HelseId.Standard.Models.Payloads;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HelseId.Standard.Tests.Mocks;

public class SigningTokenCreatorMock : ISigningTokenCreator
{
    private const string PrivateEcKey = """{ "kty": "EC", "d": "_MRDDh51lu_6KbSrJYdtMFOMwSSMo5NhTHxP_4t0ieY7YlkcDB4Ur8OUloFgLBCt", "use": "sig", "crv": "P-384", "x": "ZYQP5nGgDp6o33Q3KdnZTiWVdHCfC43Yb2b2qLyyDU9XeaJMaQB55LoPa0nTyLoC", "y": "shiLIU7DKHCwLpArDE1vsDEMr-84gUcf6u1mgmnN2hGQOXMikzN44DVhp9c9cjmQ", "alg": "ES384" }""";
    private static readonly SigningCredentials SigningCredentials = new(new JsonWebKey(PrivateEcKey), SecurityAlgorithms.EcdsaSha384);
    private const string ClientId = "4a863905-3648-43cb-a0ff-710a16f28164";
    public string Value { get; set; } = "";

    public IPayloadClaimsCreator PayloadClaimsCreator { get; private set; } = null!;
    public PayloadClaimParameters PayloadClaimParameters { get; private set; } = null!;

    private Dictionary<string, object> Claims { get; set; } = new()
    {
        { JwtRegisteredClaimNames.Iss, ClientId },
    };

    public string CreateSigningToken(IPayloadClaimsCreator payloadClaimsCreator, PayloadClaimParameters payloadClaimParameters)
    {
        PayloadClaimsCreator = payloadClaimsCreator;
        PayloadClaimParameters = payloadClaimParameters;
        
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = Claims,
            SigningCredentials = SigningCredentials,
        };

        var tokenHandler = new JsonWebTokenHandler { SetDefaultTimesOnTokenCreation = false };
        Value = tokenHandler.CreateToken(securityTokenDescriptor);
        return Value;
    }
}
