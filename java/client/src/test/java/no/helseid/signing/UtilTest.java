package no.helseid.signing;

import com.nimbusds.jose.JWSAlgorithm;
import com.nimbusds.jose.JWSSigner;
import com.nimbusds.jose.jwk.Curve;
import com.nimbusds.jose.jwk.JWK;
import com.nimbusds.jose.jwk.KeyType;
import com.nimbusds.jose.jwk.KeyUse;
import no.helseid.exceptions.HelseIdException;
import org.junit.jupiter.api.Test;

import java.security.PrivateKey;
import java.security.PublicKey;

import static org.junit.jupiter.api.Assertions.*;

class UtilTest {

  @Test
  void createJWSSignerFromKeyReferenceTest() throws HelseIdException {
    var reference = RSAKeyReference.generate(Algorithm.PS512);

    JWSSigner signer = Util.createJWSSignerFromKeyReference(reference);
    assertTrue(signer.supportedJWSAlgorithms().contains(JWSAlgorithm.PS512));
  }


  @Test
  void createJWKFromKeyReference_should_accept_RSA_algorithm() throws HelseIdException {
    var reference = RSAKeyReference.generate(Algorithm.PS512);

    JWK jwk = Util.createJWKFromKeyReference(reference);
    assertEquals(reference.getKeyId(), jwk.getKeyID());
    assertEquals(JWSAlgorithm.PS512, jwk.getAlgorithm());
    assertEquals(KeyType.RSA, jwk.getKeyType());
    assertEquals(KeyUse.SIGNATURE, jwk.getKeyUse());
  }

  @Test
  void createJWKFromKeyReference_should_accept_EC_algorithm() throws HelseIdException {
    var reference = ECKeyReference.generate(Algorithm.ES256);

    JWK jwk = Util.createJWKFromKeyReference(reference);
    assertEquals(reference.getKeyId(), jwk.getKeyID());
    assertEquals(JWSAlgorithm.ES256, jwk.getAlgorithm());
    assertEquals(KeyType.EC, jwk.getKeyType());
    assertEquals(KeyUse.SIGNATURE, jwk.getKeyUse());
  }


  @Test
  void createJWKFromKeyReference_should_reject_unsupported_algorithm() throws HelseIdException {
    KeyReference invalidKeyReference = new KeyReference() {

      @Override
      public PrivateKey getPrivateKey() {
        return null;
      }

      @Override
      public PublicKey getPublicKey() {
        return null;
      }

      @Override
      public Algorithm getAlgorithm() {
        return null;
      }

      @Override
      public String getKeyId() {
        return "";
      }
    };

    try {
      Util.createJWKFromKeyReference(invalidKeyReference);
      fail();
    } catch (HelseIdException e) {
      assertEquals("Unsupported algorithm", e.getMessage());
    }
  }

  @Test
  void findCurveForAlgorithm_should_find_valid_curves_and_reject_invalid() throws HelseIdException {
    assertEquals(Curve.P_256, Util.findCurveForAlgorithm(Algorithm.ES256));
    assertEquals(Curve.P_384, Util.findCurveForAlgorithm(Algorithm.ES384));
    assertEquals(Curve.P_521, Util.findCurveForAlgorithm(Algorithm.ES512));

    try {
      Util.findCurveForAlgorithm(Algorithm.RS256);
      fail();
    } catch (HelseIdException e) {
      assertEquals("Unsupported elliptic curve algorithm", e.getMessage());
    }
  }
}