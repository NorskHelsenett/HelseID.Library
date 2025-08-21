package no.helseid.clientassertion;

import com.nimbusds.jose.JOSEException;
import com.nimbusds.jose.JWSAlgorithm;
import com.nimbusds.jose.JWSVerifier;
import com.nimbusds.jose.crypto.RSASSAVerifier;
import com.nimbusds.jose.jwk.RSAKey;
import com.nimbusds.jose.jwk.gen.JWKGenerator;
import com.nimbusds.jose.jwk.gen.RSAKeyGenerator;
import com.nimbusds.jwt.SignedJWT;
import no.helseid.configuration.HelseIdClientConfiguration;
import no.helseid.exceptions.HelseIdConfigurationException;
import org.junit.jupiter.api.Test;

import static no.helseid.clientassertion.ClientAssertionProvider.MAX_TOKEN_LIFETIME_IN_SECONDS;
import static no.helseid.configuration.HelseIdTenantClass.SINGLETENANT;
import static org.junit.jupiter.api.Assertions.*;

class ClientAssertionProviderTest {
  private static final JWKGenerator<RSAKey> rsaKeyGenerator = new RSAKeyGenerator(2048).algorithm(JWSAlgorithm.PS256);

  @Test
  void should() throws HelseIdConfigurationException, JOSEException {
    var jwk = rsaKeyGenerator.generate();
    HelseIdClientConfiguration configuration = HelseIdClientConfiguration.create(
        jwk,
        "clientId",
        "nhn:helseid/test",
        "http://localhost:3476",
        SINGLETENANT,
        null
    );
    var clientAssertionProvider = new ClientAssertionProvider(configuration, 10);

    SignedJWT clientAssertionSignedJWT = clientAssertionProvider.createClientAssertionSignedJWT();

    JWSVerifier verifier = new RSASSAVerifier(jwk.toPublicJWK());
    assertTrue(clientAssertionSignedJWT.verify(verifier));
  }

  @Test
  void should_reject_if_token_lifetime_is_longer_than_max_lifetime() throws JOSEException {
    var jwk = rsaKeyGenerator.generate();
    HelseIdClientConfiguration configuration = HelseIdClientConfiguration.create(
        jwk,
        "clientId",
        "nhn:helseid/test",
        "http://localhost:3476",
        SINGLETENANT,
        null
    );

    try {
      new ClientAssertionProvider(configuration, MAX_TOKEN_LIFETIME_IN_SECONDS + 1);
      fail();
    } catch (Exception e) {
      assertInstanceOf(HelseIdConfigurationException.class, e);
    }
  }
}