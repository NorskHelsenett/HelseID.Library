package no.helseid.clientassertion;

import com.nimbusds.jose.JOSEException;
import com.nimbusds.jose.JOSEObjectType;
import com.nimbusds.jose.JWSAlgorithm;
import com.nimbusds.jose.JWSHeader;
import com.nimbusds.jwt.JWTClaimsSet;
import com.nimbusds.jwt.SignedJWT;
import com.nimbusds.oauth2.sdk.id.JWTID;
import no.helseid.configuration.Client;
import no.helseid.exceptions.HelseIdException;

import java.util.Date;

import static com.nimbusds.oauth2.sdk.dpop.DPoPProofFactory.MINIMAL_JTI_BYTE_LENGTH;
import static no.helseid.signing.Util.createJWSSignerFromKeyReference;

/**
 * Implementation for building client assertions
 */
public interface ClientAssertion {
  /**
   * The claim key of the assertion details
   */
  String CLAIM_ASSERTION_DETAILS = "assertion_details";

  /**
   * The lifetime of a client assertion represented in milliseconds
   */
  long TOKEN_LIFETIME_IN_MILLISECONDS = 5000L;
  /**
   * The JWT type of client authentication
   */
  JOSEObjectType CLIENT_AUTHENTICATION_JWT = new JOSEObjectType("client-authentication+jwt");

  /**
   * Creates a signed client assertion representing the client without assertion details
   *
   * @param audience the audience for the client assertion jwt
   * @param client   the relevant client
   * @return A signed client assertion
   * @throws HelseIdException Is thrown if signing fails
   */
  static SignedJWT createClientAssertionSignedJWT(String audience, Client client) throws HelseIdException {
    return createClientAssertionSignedJWT(audience, client, null);
  }

  /**
   * Creates a signed client assertion representing the client and the provided assertion details
   *
   * @param audience         the audience for the client assertion jwt
   * @param client           the relevant client
   * @param assertionDetails Containing the requested assertion details
   * @return A signed client assertion
   * @throws HelseIdException Is thrown if signing fails
   * @see AssertionDetails
   */
  static SignedJWT createClientAssertionSignedJWT(String audience, Client client, Object assertionDetails) throws HelseIdException {
    var keyReference = client.keyReference();

    if (keyReference == null) {
      throw new HelseIdException("Missing key reference for client assertion");
    }

    try {
      SignedJWT signedJWT = new SignedJWT(
          new JWSHeader.Builder(JWSAlgorithm.parse(keyReference.getAlgorithm().name()))
              .type(CLIENT_AUTHENTICATION_JWT)
              .keyID(keyReference.getKeyId())
              .build(),
          createPayload(audience, client.clientId(), assertionDetails));
      signedJWT.sign(createJWSSignerFromKeyReference(keyReference));

      return signedJWT;
    } catch (JOSEException e) {
      throw new HelseIdException("An error occurred during signing the client assertion", e);
    }
  }

  /**
   * Builds the payload of a client assertion
   *
   * @param audience         the audience for the client assertion jwt
   * @param clientId         the relevant client id
   * @param assertionDetails Containing the requested assertion details
   * @return Claim set representing the payload of a client assertion
   * @see AssertionDetails
   */
  private static JWTClaimsSet createPayload(String audience, String clientId, Object assertionDetails) {
    long currentTimeEpocMilliseconds = System.currentTimeMillis();
    JWTClaimsSet.Builder builder = new JWTClaimsSet.Builder()
        .audience(audience)
        .jwtID(new JWTID(MINIMAL_JTI_BYTE_LENGTH).getValue())
        .subject(clientId)
        .issuer(clientId)
        .issueTime(new Date(currentTimeEpocMilliseconds))
        .notBeforeTime(new Date(currentTimeEpocMilliseconds))
        .expirationTime(new Date(currentTimeEpocMilliseconds + TOKEN_LIFETIME_IN_MILLISECONDS));

    if (assertionDetails != null) {
      builder.claim(CLAIM_ASSERTION_DETAILS, assertionDetails);
    }

    return builder.build();
  }
}