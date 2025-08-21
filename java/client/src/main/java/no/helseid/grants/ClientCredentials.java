package no.helseid.grants;

import com.nimbusds.jose.JOSEException;
import com.nimbusds.jwt.SignedJWT;
import com.nimbusds.oauth2.sdk.*;
import com.nimbusds.oauth2.sdk.auth.PrivateKeyJWT;
import com.nimbusds.oauth2.sdk.dpop.DPoPProofFactory;
import com.nimbusds.oauth2.sdk.dpop.DefaultDPoPProofFactory;
import com.nimbusds.oauth2.sdk.http.HTTPRequest;
import com.nimbusds.oauth2.sdk.http.HTTPResponse;
import com.nimbusds.oauth2.sdk.token.DPoPAccessToken;
import com.nimbusds.openid.connect.sdk.Nonce;
import no.helseid.clientassertion.ClientAssertionProvider;
import no.helseid.clientassertion.HelseIdAssertionDetails;
import no.helseid.configuration.HelseIdClientConfiguration;
import no.helseid.exceptions.HelseIdConfigurationException;
import no.helseid.exceptions.HelseIdException;
import no.helseid.exceptions.HelseIdNetworkException;
import no.helseid.exceptions.HelseIdResponseException;
import no.helseid.metadata.RemoteHelseIdMetadataProvider;

import java.io.IOException;
import java.net.URI;


public class ClientCredentials {
  private static final Long DEFAULT_TOKEN_LIFETIME = 10L;
  private final HelseIdClientConfiguration configuration;
  private final DPoPProofFactory dPoPProofFactory;
  private final RemoteHelseIdMetadataProvider metadataProvider;
  private final ClientAssertionProvider jwtProvider;

  public ClientCredentials(final HelseIdClientConfiguration configuration) throws HelseIdException {
    this(
        configuration,
        createDPoPFactory(configuration),
        new RemoteHelseIdMetadataProvider(configuration.stsUri),
        new ClientAssertionProvider(configuration, DEFAULT_TOKEN_LIFETIME)
    );
  }

  public ClientCredentials(
      final HelseIdClientConfiguration configuration,
      final DPoPProofFactory dPoPProofFactory,
      final RemoteHelseIdMetadataProvider metadataProvider,
      final ClientAssertionProvider jwtProvider
  ) throws HelseIdException {
    this.configuration = configuration;
    this.dPoPProofFactory = dPoPProofFactory;
    this.metadataProvider = metadataProvider;
    this.jwtProvider = jwtProvider;

    // Fill cache early
    metadataProvider.getMetadata();
  }

  private static DPoPProofFactory createDPoPFactory(HelseIdClientConfiguration configuration) throws HelseIdException {
    try {
      return new DefaultDPoPProofFactory(configuration.jwk, configuration.algorithm);
    } catch (JOSEException e) {
      throw new HelseIdConfigurationException("An error occured while create a signer for DPoP-Proofs, please check your configuration.", e);
    }
  }

  public String getDPoPProof(String htm, URI htu, DPoPAccessToken dPoPAccessToken) throws HelseIdException {
    return createDPoPProof(htm, htu, dPoPAccessToken, null).serialize();
  }

  private SignedJWT createDPoPProof(String htm, URI htu, DPoPAccessToken dPoPAccessToken, Nonce nonce) throws HelseIdException {
    try {
      return dPoPProofFactory.createDPoPJWT(htm, htu, dPoPAccessToken, nonce);
    } catch (JOSEException e) {
      throw new HelseIdConfigurationException("An error occured while signing a DPoP-Proof, please check your configuration.", e);
    }
  }

  public AccessTokenResponse getDPoPAccessToken(HelseIdAssertionDetails assertionDetails) throws HelseIdException {
    AuthorizationGrant clientGrant = new ClientCredentialsGrant();
    var metadata = metadataProvider.getMetadata();

    /*
    // TODO: Should the library validate according to the HelseId security profile?
    if (metadata.getTokenEndpointJWSAlgs() != null && metadata.getTokenEndpointJWSAlgs().contains(configuration.algorithm)) {
      throw new IllegalArgumentException("The algorithm " + configuration.algorithm + " is not supported.");
    }
     */

    URI tokenEndpoint = metadata.getTokenEndpointURI();
    SignedJWT clientAssertion = jwtProvider.createClientAssertionSignedJWT(assertionDetails);

    return sendRequest(tokenEndpoint, new PrivateKeyJWT(clientAssertion), clientGrant, configuration.scope, null);
  }

  private AccessTokenResponse sendRequest(URI tokenEndpoint, PrivateKeyJWT privateKeyJWT, AuthorizationGrant grantType, Scope scope, Nonce dPoPNonce) throws HelseIdException {
    TokenRequest request = new TokenRequest(tokenEndpoint, privateKeyJWT, grantType, scope);
    HTTPRequest httpRequest = request.toHTTPRequest();

    var htu = httpRequest.getURI();
    var htm = httpRequest.getMethod();
    var dPoPProof = createDPoPProof(htm.name(), htu, null, dPoPNonce);
    httpRequest.setDPoP(dPoPProof);

    HTTPResponse httpResponse;
    try {
      httpResponse = httpRequest.send();
    } catch (IOException e) {
      throw new HelseIdNetworkException("Error occurred sending the request", e, false, null);
    }

    TokenResponse tokenResponse;
    try {
      tokenResponse = TokenResponse.parse(httpResponse);
    } catch (ParseException e) {
      throw new HelseIdNetworkException("Error occurred parsing the response", e, true, httpResponse.getStatusCode());
    }

    if (!tokenResponse.indicatesSuccess()) {
      ErrorObject errorObject = tokenResponse.toErrorResponse().getErrorObject();

      if (OAuth2Error.USE_DPOP_NONCE.equals(errorObject)) {
        Nonce useDPoPNonce = httpResponse.getDPoPNonce();

        if (useDPoPNonce == null) {
          throw new HelseIdResponseException("Response indicating missing nonce but none was provided.", httpResponse.getStatusCode(), errorObject);
        }

        return sendRequest(tokenEndpoint, privateKeyJWT, grantType, scope, useDPoPNonce);
      }

      throw new HelseIdResponseException("Error response from HelseID", httpResponse.getStatusCode(), errorObject);
    }

    return tokenResponse.toSuccessResponse();
  }
}
