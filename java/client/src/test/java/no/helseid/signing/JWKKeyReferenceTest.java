package no.helseid.signing;

import com.nimbusds.jose.JWSAlgorithm;
import com.nimbusds.jose.jwk.KeyUse;
import com.nimbusds.jose.jwk.RSAKey;
import no.helseid.exceptions.HelseIdException;
import org.junit.jupiter.api.Test;

import java.security.interfaces.RSAPrivateKey;
import java.security.interfaces.RSAPublicKey;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;

class JWKKeyReferenceTest {
  private static final RSAKeyReference rsaKeyReference;
  private static final ECKeyReference ecKeyReference;

  static {
    try {
      rsaKeyReference = RSAKeyReference.generate(Algorithm.PS256);
      ecKeyReference = ECKeyReference.generate(Algorithm.ES256);
    } catch (HelseIdException e) {
      throw new RuntimeException(e);
    }
  }

  @Test
  public void should_create_configuration_from_prepared_rsa_jwk() throws HelseIdException {
    var jwkString = Util.createJWKFromKeyReference(rsaKeyReference).toString();
    JWKKeyReference provider = JWKKeyReference.parse(jwkString);

    assertEquals(rsaKeyReference.getKeyId(), provider.getKeyId());
    assertEquals(rsaKeyReference.getPrivateKey(), provider.getPrivateKey());
    assertEquals(rsaKeyReference.getPublicKey(), provider.getPublicKey());
    assertEquals(rsaKeyReference.getAlgorithm(), provider.getAlgorithm());
  }


  @Test
  public void should_create_configuration_from_prepared_ec_jwk() throws HelseIdException {
    var jwkString = Util.createJWKFromKeyReference(ecKeyReference).toString();
    JWKKeyReference provider = JWKKeyReference.parse(jwkString);

    assertEquals(ecKeyReference.getKeyId(), provider.getKeyId());
    assertEquals(ecKeyReference.getPrivateKey(), provider.getPrivateKey());
    assertEquals(ecKeyReference.getPublicKey(), provider.getPublicKey());
    assertEquals(ecKeyReference.getAlgorithm(), provider.getAlgorithm());
  }

  @Test
  public void should_reject_configuration_if_missing_algorithm() {
    try {
      var jwkString = new RSAKey.Builder((RSAPublicKey) rsaKeyReference.getPublicKey())
          .privateKey((RSAPrivateKey) rsaKeyReference.getPrivateKey())
          /* .algorithm(JWSAlgorithm.PS256) */
          .keyUse(KeyUse.SIGNATURE)
          .keyID(UUID.randomUUID().toString())
          .build()
          .toJSONString();

      JWKKeyReference.parse(jwkString);
      fail();
    } catch (HelseIdException e) {
      assertTrue(e.getMessage().contains("Missing algorithm in jwk"));
    }
  }


  @Test
  public void should_reject_configuration_if_missing_key_id() {
    try {
      var jwkString = new RSAKey.Builder((RSAPublicKey) rsaKeyReference.getPublicKey())
          .privateKey((RSAPrivateKey) rsaKeyReference.getPrivateKey())
          .algorithm(JWSAlgorithm.PS256)
          .keyUse(KeyUse.SIGNATURE)
          /* .keyID(keyId) */
          .build()
          .toJSONString();

      JWKKeyReference.parse(jwkString);
      fail();
    } catch (HelseIdException e) {
      assertEquals("Missing key-id in jwk", e.getMessage());
    }
  }

  @Test
  public void should_reject_configuration_jwt_with_bad_string_representation() {
    try {
      var jwkString = Util.createJWKFromKeyReference(rsaKeyReference).toString();

      JWKKeyReference.parse(jwkString.substring(0, jwkString.length() - 1));
      fail();
    } catch (HelseIdException e) {
      assertEquals("Bad string representation of a jwk", e.getMessage());
    }
  }
}