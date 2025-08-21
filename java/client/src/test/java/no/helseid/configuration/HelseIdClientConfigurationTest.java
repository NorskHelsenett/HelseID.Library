package no.helseid.configuration;

import com.nimbusds.jose.JWSAlgorithm;
import com.nimbusds.jose.jwk.JWK;
import no.helseid.exceptions.HelseIdConfigurationException;
import org.junit.jupiter.api.Test;

import java.text.ParseException;
import java.util.Collections;
import java.util.Properties;

import static no.helseid.configuration.HelseIdTenantClass.SINGLETENANT;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertInstanceOf;

class HelseIdClientConfigurationTest {
  final static String generalPrivateEcKey = """
      {
          "kty": "EC",
          "d": "PTQlsiXQ-PU_aeG1cSXZmEtm_rJH7Q5lEtqn9hP-SOlNHurT3vpM6gMy28h59G8u",
          "use": "sig",
          "crv": "P-384",
          "x": "_fwQ_E2rqeBOQ0YYzQBCvZNK60-n4PUG7cbJelBkuAbfEqmnaMHNUsReIsnE3432",
          "y": "xbuUzn7GpWq7JuKgrY_QxskViWPyDk_MoIef5JXXPlWkdB24cQLVgm-Jgz8NOblZ",
          "alg": "ES384",
          "kid": "kidvalue"
      }
      """;
  final static String invalidPrivateKey = """
      {
          "kty": "OKP",
          "d": "bVM7WDQzTQ4Rl5cxZwZ9kKgExYg72lKhiJ4GwTnv-kQ",
          "use": "sig",
          "crv": "X25519",
          "x": "wi4qHzZaqP4izZBQZYZ8Chi45hJohzmQUjxaVt-SiXI",
          "alg": "EdDSA"
      }
      """;

  final static String clientId = "294c7ab7-a22e-4a79-aea3-34edddf5b85d";
  final static String scope = "openid profile offline_access";
  final static String stsUrl = "https://helseid-sts.test.nhn.no";

  @Test
  public void should_create_configuration_from_prepared_jwk() throws ParseException {
    JWK jsonWebKey = JWK.parse(generalPrivateEcKey);

    var configuration = HelseIdClientConfiguration.create(jsonWebKey, clientId, scope, stsUrl, SINGLETENANT, Collections.emptyList());

    assertEquals(configuration.resourceIndicators.size(), 0);
    assertEquals(JWSAlgorithm.ES384, configuration.jwk.getAlgorithm());
  }

  @Test
  public void should_create_configuration_from_string_jwk() throws HelseIdConfigurationException {
    var configuration = HelseIdClientConfiguration.create(generalPrivateEcKey, clientId, scope, stsUrl, SINGLETENANT, Collections.emptyList());

    assertEquals(JWSAlgorithm.ES384, configuration.jwk.getAlgorithm());
  }

  @Test
  public void should_reject_invalid_string_jwk() {
    try {
      HelseIdClientConfiguration.create(invalidPrivateKey, clientId, scope, stsUrl, SINGLETENANT, Collections.emptyList());
    } catch (Exception e) {
      assertInstanceOf(HelseIdConfigurationException.class, e);
    }
  }

  @Test
  public void should_create_configuration_from_properties() throws HelseIdConfigurationException {
    var properties = new Properties();
    properties.put("helseid.clientid", "client id");
    properties.put("helseid.stsurl", "https://sts.org");
    properties.put("helseid.scope", "scope");
    properties.put("helseid.jwk", generalPrivateEcKey);

    var configuration = HelseIdClientConfiguration.create(properties);

    assertEquals("client id", configuration.clientId.toString());
    assertEquals("https://sts.org", configuration.stsUri.toString());
    assertEquals("scope", configuration.scope.toString());
    assertEquals(JWSAlgorithm.ES384, configuration.jwk.getAlgorithm());
    assertEquals("kidvalue", configuration.jwk.getKeyID());
  }

  @Test
  public void should_create_configuration_from_properties_with_algorithm_specified() throws HelseIdConfigurationException {
    var properties = new Properties();
    properties.put("helseid.clientid", "client id");
    properties.put("helseid.stsurl", "https://sts.org");
    properties.put("helseid.scope", "scope");
    properties.put("helseid.algorithm", "ES256");
    properties.put("helseid.jwk", generalPrivateEcKey);

    var configuration = HelseIdClientConfiguration.create(properties);

    assertEquals("client id", configuration.clientId.toString());
    assertEquals("https://sts.org", configuration.stsUri.toString());
    assertEquals("scope", configuration.scope.toString());
    assertEquals(JWSAlgorithm.ES384, configuration.jwk.getAlgorithm());
    assertEquals("kidvalue", configuration.jwk.getKeyID());
  }
}