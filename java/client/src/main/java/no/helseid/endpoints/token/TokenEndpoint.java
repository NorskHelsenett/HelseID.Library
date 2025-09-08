package no.helseid.endpoints.token;

import com.nimbusds.jwt.SignedJWT;
import com.nimbusds.oauth2.sdk.*;
import com.nimbusds.oauth2.sdk.auth.PrivateKeyJWT;
import com.nimbusds.oauth2.sdk.http.HTTPRequest;
import com.nimbusds.oauth2.sdk.http.HTTPResponse;
import com.nimbusds.oauth2.sdk.token.AccessTokenType;
import com.nimbusds.oauth2.sdk.token.DPoPAccessToken;
import com.nimbusds.oauth2.sdk.token.Tokens;
import com.nimbusds.openid.connect.sdk.Nonce;
import no.helseid.dpop.DPoPProofCreator;
import no.helseid.dpop.HttpMethod;
import no.helseid.exceptions.HelseIdException;

import java.io.IOException;
import java.net.URI;
import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.Optional;

/**
 * A util for performing requests to the token endpoint
 */
public interface TokenEndpoint {

  /**
   * Sends a request to a token endpoint
   * @param tokenEndpointURI the token endpoint
   * @param dpopProofCreator a DPoP-proof creator
   * @param clientAssertion a client assertion
   * @param grantType the grant requested
   * @param scope all scopes requested
   * @param resources resource indicators for the token
   * @param customParams optional additional parameters
   * @return a Token Response
   * @throws HelseIdException if an error occurs not representable by an ErrorResponse
   */
  static TokenResponse sendRequest(
      URI tokenEndpointURI,
      DPoPProofCreator dpopProofCreator,
      SignedJWT clientAssertion,
      AuthorizationGrant grantType,
      Scope scope,
      List<URI> resources,
      Map<String, List<String>> customParams) throws HelseIdException {
    TokenRequest request = new TokenRequest(
        tokenEndpointURI,
        new PrivateKeyJWT(clientAssertion),
        grantType,
        scope,
        null,
        resources,
        null,
        customParams
    );
    HTTPResponse httpResponseWithoutIncludingDPoPNonceResponseHeader = sendTokenRequest(request, dpopProofCreator, null);
    com.nimbusds.oauth2.sdk.TokenResponse initialTokenResponse = parseTokenResponse(httpResponseWithoutIncludingDPoPNonceResponseHeader);

    // Should not happen because of DPoP
    if (initialTokenResponse.indicatesSuccess()) {
      return handleSuccess(initialTokenResponse.toSuccessResponse().getTokens());
    }

    ErrorObject errorObjectWithoutDPoPNonce = initialTokenResponse.toErrorResponse().getErrorObject();

    if (!OAuth2Error.USE_DPOP_NONCE.equals(errorObjectWithoutDPoPNonce)) {
      return handleError(errorObjectWithoutDPoPNonce);
    }

    Nonce dPoPNonce = httpResponseWithoutIncludingDPoPNonceResponseHeader.getDPoPNonce();
    if (dPoPNonce == null) {
      throw new HelseIdException("Response indicating missing nonce but none was provided.");
    }

    HTTPResponse httpResponse = sendTokenRequest(request, dpopProofCreator, dPoPNonce.getValue());
    com.nimbusds.oauth2.sdk.TokenResponse dPoPTokenResponse = parseTokenResponse(httpResponse);

    if (dPoPTokenResponse.indicatesSuccess()) {
      return handleSuccess(dPoPTokenResponse.toSuccessResponse().getTokens());
    }

    return handleError(dPoPTokenResponse.toErrorResponse().getErrorObject());
  }

  /**
   * Parses the token response and converts it to a HelseID AccessTokenResponse
   * @param tokens the nimbus token response
   * @return a HelseID AccessTokenResponse
   */
  private static AccessTokenResponse handleSuccess(Tokens tokens) {
    DPoPAccessToken accessToken = tokens.getDPoPAccessToken();

    return new AccessTokenResponse(
        accessToken.toString(),
        Optional.ofNullable(accessToken.getType()).orElse(AccessTokenType.UNKNOWN).getValue(),
        accessToken.getLifetime(),
        Optional.ofNullable(accessToken.getScope()).map(Scope::toStringList).orElse(Collections.emptyList())
    );
  }

  private static ErrorResponse handleError(ErrorObject errorObject) {
    return new ErrorResponse(
        errorObject.getCode(),
        errorObject.getDescription(),
        errorObject.getHTTPStatusCode(),
        errorObject.toHTTPResponse().getBody()
    );
  }

  /**
   * Internal util class for sending token request and handle exception during the process
   * @param tokenRequest a token request object
   * @param dPoPProofCreator a DPoP-proof creator
   * @param dPoPNonce an optional DPoP-Nonce
   * @return a nimbus HTTP Response
   * @throws HelseIdException if the request is not sent off or the response is not processable
   */
  private static HTTPResponse sendTokenRequest(TokenRequest tokenRequest, DPoPProofCreator dPoPProofCreator, String dPoPNonce) throws HelseIdException {
    HTTPRequest httpRequest = tokenRequest.toHTTPRequest();
    var htu = httpRequest.getURI();
    var htm = httpRequest.getMethod();
    var dPoPProof = dPoPProofCreator.createDPoPProofWithNonce(htu, new HttpMethod(htm.name()), dPoPNonce);
    httpRequest.setHeader("DPoP", dPoPProof);

    HTTPResponse httpResponse;
    try {
      return httpRequest.send();
    } catch (IOException e) {
      throw new HelseIdException("Error occurred sending the request", e);
    }
  }

  /**
   * Reads a generic HTTP Response to a token response
   * NB: This will remove the DPoP-Nonce header
   * @param httpResponse the HTTP response to be parsed
   * @return a nimbus token response
   * @throws HelseIdException it the response is unprocessable
   */
  private static com.nimbusds.oauth2.sdk.TokenResponse parseTokenResponse(HTTPResponse httpResponse) throws HelseIdException {
    try {
      return com.nimbusds.oauth2.sdk.TokenResponse.parse(httpResponse);
    } catch (ParseException e) {
      var message = String.format(
          "Bad format on token response, HTTP status: %d. Content length %d",
          httpResponse.getStatusCode(),
          Optional.ofNullable(httpResponse.getBody())
              .map(String::length)
              .orElse(0));
      throw new HelseIdException(message, e);
    }
  }
}
