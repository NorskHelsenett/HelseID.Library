package no.helseid.configuration;

import com.nimbusds.jose.JWSAlgorithm;
import com.nimbusds.jose.jwk.JWK;
import com.nimbusds.oauth2.sdk.Scope;
import com.nimbusds.oauth2.sdk.id.ClientID;
import no.helseid.exceptions.HelseIdConfigurationException;

import java.net.URI;
import java.text.ParseException;
import java.util.Collections;
import java.util.List;
import java.util.Properties;

public class HelseIdClientConfiguration extends HelseIdConfiguration {
  public final JWK jwk;
  public final JWSAlgorithm algorithm;
  public final ClientID clientId;
  public final Scope scope;
  // Multitenant
  // These are used for clients that are using resource indicators against the PAR and token endpoints:
  public final HelseIdTenantClass tenantClass;
  public List<String> resourceIndicators;

  HelseIdClientConfiguration(
      JWK jwk,
      JWSAlgorithm algorithm,
      ClientID clientId,
      Scope scope,
      URI stsUri,
      HelseIdTenantClass tenantClass,
      List<String> resourceIndicators
  ) {
    super(stsUri);
    this.scope = scope;
    this.clientId = clientId;
    this.jwk = jwk;
    this.algorithm = algorithm;
    this.tenantClass = tenantClass;
    this.resourceIndicators = resourceIndicators == null ? Collections.emptyList() : resourceIndicators;
  }

  public static HelseIdClientConfiguration create(
      JWK jwk,
      String clientId,
      String scope,
      String stsUrl,
      HelseIdTenantClass tenantClass,
      List<String> resourceIndicators,
      String algorithm) {
    return new HelseIdClientConfiguration(
        jwk,
        JWSAlgorithm.parse(algorithm),
        new ClientID(clientId),
        new Scope(scope.split(" ")),
        URI.create(stsUrl),
        tenantClass,
        resourceIndicators
    );
  }

  public static HelseIdClientConfiguration create(
      JWK jwk,
      String clientId,
      String scope,
      String stsUrl,
      HelseIdTenantClass tenantClass,
      List<String> resourceIndicators) {
    return create(jwk, clientId, scope, stsUrl, tenantClass, resourceIndicators, jwk.getAlgorithm().getName());
  }

  public static HelseIdClientConfiguration create(
      String jsonWebKey,
      String clientId,
      String scope,
      String stsUrl,
      HelseIdTenantClass tenantClass,
      List<String> resourceIndicators) throws HelseIdConfigurationException {
    return create(parseJWK(jsonWebKey), clientId, scope, stsUrl, tenantClass, resourceIndicators);
  }

  public static HelseIdClientConfiguration create(
      String jsonWebKey,
      String clientId,
      String scope,
      String stsUrl,
      HelseIdTenantClass tenantClass,
      List<String> resourceIndicators,
      String algorithm) throws HelseIdConfigurationException {
    return create(parseJWK(jsonWebKey), clientId, scope, stsUrl, tenantClass, resourceIndicators, algorithm);
  }

  public static HelseIdClientConfiguration create(Properties properties) throws HelseIdConfigurationException {
    var clientId = properties.getProperty("helseid.clientid");
    var stsUrl = properties.getProperty("helseid.stsurl");
    var scope = properties.getProperty("helseid.scope");
    var tenantClass = HelseIdTenantClass.valueOf(properties.getProperty("helseid.tenant"));
    var jwk = properties.getProperty("helseid.jwk");

    return create(jwk, clientId, scope, stsUrl, tenantClass, null);
  }

  private static JWK parseJWK(String jsonWebKey) throws HelseIdConfigurationException {
    try {
      return JWK.parse(jsonWebKey);
    } catch (ParseException e) {
      throw new HelseIdConfigurationException("Bad string representation of a jwk", e);
    }
  }
}
