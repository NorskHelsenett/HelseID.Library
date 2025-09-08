package no.helseid.dpop;

import com.nimbusds.jose.JOSEException;
import com.nimbusds.jose.JWSAlgorithm;
import com.nimbusds.jose.jwk.JWK;
import com.nimbusds.jose.util.Base64URL;
import com.nimbusds.jwt.SignedJWT;
import com.nimbusds.oauth2.sdk.dpop.DPoPProofFactory;
import com.nimbusds.oauth2.sdk.dpop.DefaultDPoPProofFactory;
import com.nimbusds.oauth2.sdk.dpop.JWKThumbprintConfirmation;
import com.nimbusds.oauth2.sdk.token.DPoPAccessToken;
import com.nimbusds.openid.connect.sdk.Nonce;
import no.helseid.exceptions.HelseIdException;
import no.helseid.signing.KeyReference;

import java.net.URI;
import java.text.ParseException;

import static no.helseid.signing.Util.createJWKFromKeyReference;

/**
 * Default implementation of a DPoPProofCreator
 */
public class DefaultDPoPProofCreator implements DPoPProofCreator {
  private final DPoPProofFactory factory;
  private final String keyId;
  private final Base64URL thumbprint;

  /**
   * Create a default implementation of the DPoPProofCreator
   * @param keyReference a key reference
   * @throws HelseIdException  if unable to create a creator
   */
  public DefaultDPoPProofCreator(KeyReference keyReference) throws HelseIdException {
    try {
      JWK jwk = createJWKFromKeyReference(keyReference);
      this.factory = new DefaultDPoPProofFactory(jwk, JWSAlgorithm.parse(jwk.getAlgorithm().getName()));
      this.keyId = jwk.getKeyID();
      this.thumbprint = jwk.computeThumbprint();
    } catch (JOSEException e) {
      throw new HelseIdException("Failed to create factory", e);
    }
  }

  /**
   * Creating a DPoP-Proof with nonce, proving possession of a private key
   *
   * @param htu   - Request URL
   * @param htm   - Request Method
   * @param nonce - Nonce provided by HelseId
   * @return string formatted DPoP-Proof
   * @throws HelseIdException if unable to create a DPoP-Proof
   */
  public String createDPoPProofWithNonce(URI htu, HttpMethod htm, String nonce) throws HelseIdException {
    return createDPoPProof(htu, htm, nonce, null);
  }

  /**
   * Creating a DPoP-Proof with access token, proving intent of token usage
   *
   * @param htu   - Request URL
   * @param htm   - Request Method
   * @param accessToken - DPoP Access token bound by a private key
   * @return string formatted DPoP-Proof
   * @throws HelseIdException if unable to create a DPoP-Proof, usually the thumbprint in the access token does not match the signing key
   */
  public String createDPoPProof(URI htu, HttpMethod htm, String accessToken) throws HelseIdException {
    return createDPoPProof(htu, htm, null, accessToken);
  }

  /**
   * Creating a DPoP-Proof
   *
   * @param htu   - Request URL
   * @param htm   - Request Method
   * @param nonce - Nonce provided by HelseId
   * @param accessToken - DPoP Access token bound by a private key
   * @return string formatted DPoP-Proof
   * @throws HelseIdException if unable to create a DPoP-Proof, usually the thumbprint in the access token does not match the signing key
   */
  private String createDPoPProof(URI htu, HttpMethod htm, String nonce, String accessToken) throws HelseIdException {
    if (accessToken != null) {
      try {
        Base64URL accessTokenJwkThumbprint = JWKThumbprintConfirmation.parse(SignedJWT.parse(accessToken).getJWTClaimsSet()).getValue();

        if (!thumbprint.equals(accessTokenJwkThumbprint)) {
          throw new HelseIdException("The JWK thumbprint in the access token does not match the thumbprint of the private key JWK");
        }

      } catch (ParseException e) {
        throw new HelseIdException("Unable to parse access token", e);
      }
    }

    try {
      var dPoPAccessToken = accessToken == null ? null : new DPoPAccessToken(accessToken);
      return factory.createDPoPJWT(htm.value, htu, dPoPAccessToken, Nonce.parse(nonce)).serialize();
    } catch (JOSEException e) {
      throw new HelseIdException("Unable to create DPoP proof", e);
    }
  }

  @Override
  public String getKeyId() {
    return keyId;
  }
}
