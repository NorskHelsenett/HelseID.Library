package no.helseid.clientassertion;

import com.nimbusds.jose.JOSEException;
import com.nimbusds.jose.JWSAlgorithm;
import com.nimbusds.jose.JWSVerifier;
import com.nimbusds.jose.crypto.RSASSAVerifier;
import com.nimbusds.jose.jwk.KeyUse;
import com.nimbusds.jose.jwk.RSAKey;
import com.nimbusds.jose.jwk.gen.JWKGenerator;
import com.nimbusds.jose.jwk.gen.RSAKeyGenerator;
import com.nimbusds.jwt.SignedJWT;
import no.helseid.configuration.Client;
import no.helseid.exceptions.HelseIdException;
import no.helseid.signing.JWKKeyReference;
import org.junit.jupiter.api.Test;

import java.util.Collections;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;

class ClientAssertionTest {
  private static final JWKGenerator<RSAKey> rsaKeyGenerator = new RSAKeyGenerator(2048)
      .keyUse(KeyUse.SIGNATURE)
      .algorithm(JWSAlgorithm.PS256);
  private static final String AUTHORITY = "http://localhost:3476";
  private final RSAKey rsaKey;
  public ClientAssertionTest() throws JOSEException {
    rsaKey = rsaKeyGenerator.keyID(UUID.randomUUID().toString()).generate();
  }

  @Test
  void should_create_client_assertion_with_signature_based_on_private_key_jwk() throws JOSEException, HelseIdException {
    Client client = new Client(
        "clientId",
        JWKKeyReference.parse(rsaKey.toJSONString()),
        Collections.singletonList("nhn:helseid/test")
    );
    SignedJWT clientAssertionSignedJWT = ClientAssertion.createClientAssertionSignedJWT(AUTHORITY, client);

    JWSVerifier verifier = new RSASSAVerifier(rsaKey);
    assertTrue(clientAssertionSignedJWT.verify(verifier));
  }

  @Test
  void should_fail_if_client_is_missing_key_reference() {
    Client client = new Client(
        "clientId",
        null,
        Collections.singletonList("nhn:helseid/test")
    );
    try {
      ClientAssertion.createClientAssertionSignedJWT(AUTHORITY, client);
      fail();
    } catch (HelseIdException e) {
      assertEquals("Missing key reference for client assertion", e.getMessage());
    }
  }
}