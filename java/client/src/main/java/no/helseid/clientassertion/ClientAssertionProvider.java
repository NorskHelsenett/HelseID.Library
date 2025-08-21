package no.helseid.clientassertion;

import com.nimbusds.jose.JOSEException;
import com.nimbusds.jose.JOSEObjectType;
import com.nimbusds.jose.JWSHeader;
import com.nimbusds.jose.JWSSigner;
import com.nimbusds.jose.crypto.factories.DefaultJWSSignerFactory;
import com.nimbusds.jwt.JWTClaimsSet;
import com.nimbusds.jwt.SignedJWT;
import no.helseid.cache.HelseIdCache;
import no.helseid.cache.HelseIdInMemoryCache;
import no.helseid.configuration.HelseIdClientConfiguration;
import no.helseid.exceptions.HelseIdConfigurationException;

import java.time.Instant;
import java.util.*;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import static no.helseid.configuration.HelseIdTenantClass.MULTITENANT;
import static no.helseid.configuration.HelseIdTenantClass.SINGLETENANT;

public class ClientAssertionProvider {
  static final String CLAIM_ASSERTION_DETAILS = "assertion_details";
  static final long MAX_TOKEN_LIFETIME_IN_SECONDS = 60L;
  private static final JOSEObjectType CLIENT_AUTHENTICATION_JWT = new JOSEObjectType("client-authentication+jwt");
  private final HelseIdCache<SignedJWT> clientAuthenticationJWTCache;
  private final HelseIdClientConfiguration configuration;
  private final long tokenLifetimeInSeconds;

  public ClientAssertionProvider(
      HelseIdClientConfiguration configuration,
      long tokenLifetimeInSeconds
  ) throws HelseIdConfigurationException {

    // TODO: Should the library validate according to the HelseId security profile?
    if (tokenLifetimeInSeconds > MAX_TOKEN_LIFETIME_IN_SECONDS) {
      throw new HelseIdConfigurationException("The lifetime of a cient assertion cannot exceed " + MAX_TOKEN_LIFETIME_IN_SECONDS + " seconds. Ideally the lifetime should be much shorter.");
    }
    this.tokenLifetimeInSeconds = tokenLifetimeInSeconds;
    this.clientAuthenticationJWTCache = new HelseIdInMemoryCache<>(tokenLifetimeInSeconds / 2 * 1000);
    this.configuration = configuration;
  }

  public SignedJWT createClientAssertionSignedJWT() throws HelseIdConfigurationException {
    return createClientAssertionSignedJWT(null);
  }

  public SignedJWT createClientAssertionSignedJWT(HelseIdAssertionDetails assertionDetails) throws HelseIdConfigurationException {
    var cacheKey = createCacheKey(assertionDetails);
    var signedJWT = clientAuthenticationJWTCache.get(cacheKey);

    if (signedJWT == null) {
      signedJWT = createPrivateKeyJWT(assertionDetails);
      clientAuthenticationJWTCache.put(cacheKey, signedJWT);
    }

    return signedJWT;
  }

  private SignedJWT createPrivateKeyJWT(HelseIdAssertionDetails assertionDetails) throws HelseIdConfigurationException {
    try {
      DefaultJWSSignerFactory factory = new DefaultJWSSignerFactory();
      JWSSigner signer = factory.createJWSSigner(configuration.jwk);

      SignedJWT signedJWT = new SignedJWT(
          new JWSHeader.Builder(configuration.algorithm)
              .type(CLIENT_AUTHENTICATION_JWT)
              .keyID(configuration.jwk.getKeyID())
              .build(),
          buildClaims(tokenLifetimeInSeconds, assertionDetails));
      signedJWT.sign(signer);

      return signedJWT;
    } catch (JOSEException e) {
      throw new HelseIdConfigurationException("An error occurred during signing the client assertion", e);
    }
  }

  private JWTClaimsSet buildClaims(long tokenLifetimeInSeconds, HelseIdAssertionDetails assertionDetails) {
    JWTClaimsSet.Builder builder = new JWTClaimsSet.Builder()
        .audience(configuration.stsUri.toString())
        .jwtID(UUID.randomUUID().toString())
        .subject(configuration.clientId.toString())
        .issuer(configuration.clientId.toString())
        .issueTime(new Date())
        .notBeforeTime(new Date())
        .expirationTime(Date.from(Instant.now().plusSeconds(tokenLifetimeInSeconds)));

    if (assertionDetails != null) {
      builder.claim(CLAIM_ASSERTION_DETAILS, createAssertionDetailsClaims(assertionDetails));
    }

    return builder.build();
  }

  private String createCacheKey(HelseIdAssertionDetails assertionDetails) {
    if (assertionDetails == null) {
      return "CLIENT"; // TODO: Is it ok to create a default key when no assertion detail is provided?
    }

    return Stream.of(
            assertionDetails.parentOrganizationNumber,
            assertionDetails.childOrganizationNumber,
            assertionDetails.sfmJournalId
        )
        .filter(Objects::nonNull)
        .collect(Collectors.joining(":"));
  }

  private List<Map<String, Object>> createAssertionDetailsClaims(HelseIdAssertionDetails assertionDetails) {
    ArrayList<Map<String, Object>> assertionDetailsClaims = new ArrayList<>();

    if (SINGLETENANT.equals(configuration.tenantClass) && assertionDetails.childOrganizationNumber != null) {
      assertionDetailsClaims.add(ClaimsUtil.createClaimHelseidAuthorizationForSingleTenant(assertionDetails.childOrganizationNumber));
    }
    if (MULTITENANT.equals(configuration.tenantClass) && assertionDetails.parentOrganizationNumber != null) {
      assertionDetailsClaims.add(ClaimsUtil.createClaimHelseidAuthorizationForMultiTenant(assertionDetails.parentOrganizationNumber, assertionDetails.childOrganizationNumber));
    }

    if (assertionDetails.sfmJournalId != null) {
      assertionDetailsClaims.add(ClaimsUtil.createClaimSFMJournalId(assertionDetails.sfmJournalId));
    }

    return assertionDetailsClaims;
  }
}