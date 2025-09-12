package no.helseid.dpop;

import com.nimbusds.jose.JOSEException;
import com.nimbusds.jose.JOSEObjectType;
import com.nimbusds.jose.JWSAlgorithm;
import com.nimbusds.jose.JWSHeader;
import com.nimbusds.jose.util.Base64URL;
import com.nimbusds.jwt.JWTClaimsSet;
import com.nimbusds.jwt.SignedJWT;
import com.nimbusds.oauth2.sdk.id.JWTID;
import com.nimbusds.openid.connect.sdk.Nonce;
import no.helseid.exceptions.HelseIdException;
import no.helseid.signing.Algorithm;
import no.helseid.signing.KeyReference;
import no.helseid.signing.RSAKeyReference;
import no.helseid.signing.Util;
import org.junit.jupiter.api.Test;

import java.net.URI;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.text.ParseException;
import java.util.Date;
import java.util.Map;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;

public class DefaultDPoPProofCreatorTest {
  private static final KeyReference SIGNING_KEY_REFERENCE;
  private static final URI HTU = URI.create("https://helseid.no");

  static {
    try {
      SIGNING_KEY_REFERENCE = RSAKeyReference.generate(Algorithm.PS256);
    } catch (HelseIdException e) {
      throw new RuntimeException(e);
    }
  }

  @Test
  public void should_withKeyReference_a_DPoPProofCreator_based_on_KeyReferencee() throws HelseIdException {
    var keyReference = RSAKeyReference.generate(Algorithm.PS256);
    DPoPProofCreator proofCreator = new DefaultDPoPProofCreator(keyReference);

    assertEquals(keyReference.getKeyId(), proofCreator.getKeyId());
  }

  @Test
  public void should_initialize_a_dpop_proof_containing_provided_nonce() throws HelseIdException, ParseException {
    var keyReference = RSAKeyReference.generate(Algorithm.PS256);
    DPoPProofCreator proofCreator = new DefaultDPoPProofCreator(keyReference);
    String originalNonce = new Nonce().toString();
    String dPoPProofWithNonce = proofCreator.createDPoPProofWithNonce(HTU, HttpMethod.GET, originalNonce);
    SignedJWT parsedProof = SignedJWT.parse(dPoPProofWithNonce);
    JWTClaimsSet claims = parsedProof.getJWTClaimsSet();
    String nonceFromClaim = claims.getStringClaim("nonce");
    assertEquals(originalNonce, nonceFromClaim);
  }

  @Test
  public void should_initialize_a_dpop_proof_bound_to_token() throws HelseIdException, ParseException, NoSuchAlgorithmException {
    var keyReference = RSAKeyReference.generate(Algorithm.PS256);
    DPoPProofCreator proofCreator = new DefaultDPoPProofCreator(keyReference);
    var dpopAccessToken = createMockDPoPAccessToken(keyReference);
    Base64URL ath = Base64URL.encode(MessageDigest.getInstance("SHA-256").digest(dpopAccessToken.getBytes(StandardCharsets.UTF_8)));

    String dPoPProofBoundToToken = proofCreator.createDPoPProof(HTU, HttpMethod.GET, dpopAccessToken);

    SignedJWT parsedProof = SignedJWT.parse(dPoPProofBoundToToken);
    JWTClaimsSet claims = parsedProof.getJWTClaimsSet();

    String athFromClaim = claims.getStringClaim("ath");
    assertEquals(ath.toString(), athFromClaim);
  }


  @Test
  public void should_throw_exception_if_token_has_bad_format() throws HelseIdException {
    var keyReference = RSAKeyReference.generate(Algorithm.PS256);
    DPoPProofCreator proofCreator = new DefaultDPoPProofCreator(keyReference);
    var dpopAccessToken = createMockDPoPAccessToken(keyReference);

    try {
      proofCreator.createDPoPProof(HTU, HttpMethod.GET, dpopAccessToken.replaceAll("\\.", ","));
      fail();
    } catch (HelseIdException e) {
      assertEquals("Unable to parse access token", e.getMessage());
    }
  }


  @Test
  public void should_thow_exception_if_key_in_token_and_dpop_creator_does_not_match() throws HelseIdException {
    DPoPProofCreator proofCreator = new DefaultDPoPProofCreator(RSAKeyReference.generate(Algorithm.PS256));
    var dpopAccessToken = createMockDPoPAccessToken(RSAKeyReference.generate(Algorithm.PS256));

    try {
      proofCreator.createDPoPProof(HTU, HttpMethod.GET, dpopAccessToken);
      fail();
    } catch (HelseIdException e) {
      assertEquals("The JWK thumbprint in the access token does not match the thumbprint of the private key JWK", e.getMessage());
    }
  }

  /**
   * Method creating mock DPoP Access tokens bound to a dpop key reference
   */
  private String createMockDPoPAccessToken(KeyReference dpopKeyReference) throws HelseIdException {

    String thumbprint;
    try {
      thumbprint = Util.createJWKFromKeyReference(dpopKeyReference).computeThumbprint().toString();

      long currentTimeEpocMilliseconds = System.currentTimeMillis();
      JWTClaimsSet payload = new JWTClaimsSet.Builder()
          .audience("nhn:helseid")
          .jwtID(new JWTID().getValue())
          .subject(UUID.randomUUID().toString())
          .issuer("https://test.helseid.no")
          .issueTime(new Date(currentTimeEpocMilliseconds))
          .notBeforeTime(new Date(currentTimeEpocMilliseconds))
          .expirationTime(new Date(currentTimeEpocMilliseconds + 1000))
          .claim("cnf", Map.of("jkt", thumbprint))
          .build();

      SignedJWT signedJWT = new SignedJWT(
          new JWSHeader.Builder(JWSAlgorithm.parse(SIGNING_KEY_REFERENCE.getAlgorithm().name()))
              .type(new JOSEObjectType("dpop+jwt"))
              .keyID(SIGNING_KEY_REFERENCE.getKeyId())
              .build(),
          payload);
      signedJWT.sign(Util.createJWSSignerFromKeyReference(SIGNING_KEY_REFERENCE));

      return signedJWT.serialize();
    } catch (JOSEException e) {
      throw new HelseIdException("Unable to create token", e);
    }
  }
}
