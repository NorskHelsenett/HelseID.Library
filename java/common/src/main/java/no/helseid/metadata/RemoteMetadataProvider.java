package no.helseid.metadata;

import com.nimbusds.oauth2.sdk.GeneralException;
import com.nimbusds.oauth2.sdk.id.Issuer;
import com.nimbusds.openid.connect.sdk.op.OIDCProviderMetadata;
import no.helseid.cache.ExpiringCache;
import no.helseid.cache.InMemoryExpiringCache;
import no.helseid.exceptions.HelseIdException;

import java.io.IOException;
import java.net.URI;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.TimeUnit;

/**
 * Provider of OIDCProviderMetadata from a discovery endpoint.
 * By default the metadata is cached for 24 hours
 *
 * @version 2025-08-25
 */
public class RemoteMetadataProvider implements MetadataProvider {
  private static final Map<String, RemoteMetadataProvider> instanceMap = new ConcurrentHashMap<>();
  private static final String CACHE_KEY = "metadata";
  private static final long MILLISECONDS_IN_A_DAY = TimeUnit.DAYS.toMillis(1);
  private final URI authority;
  private final ExpiringCache<OIDCProviderMetadata> cache;
  private final long expirationTimeInMilliseconds;

  RemoteMetadataProvider(URI authority) {
    this(authority, MILLISECONDS_IN_A_DAY, new InMemoryExpiringCache<>());
  }

  RemoteMetadataProvider(URI authority, long expirationTimeInMilliseconds) {
    this(authority, expirationTimeInMilliseconds, new InMemoryExpiringCache<>());
  }

  RemoteMetadataProvider(URI authority, long expirationTimeInMilliseconds, ExpiringCache<OIDCProviderMetadata> cache) {
    this.authority = authority;
    this.cache = cache;
    this.expirationTimeInMilliseconds = expirationTimeInMilliseconds;
  }

  /**
   * Get the cached metadata, updated metadata is fetched if the cache is expired
   *
   * @throws HelseIdException if a refresh fails
   * @return metadata object
   */
  @Override
  public OIDCProviderMetadata getMetadata() throws HelseIdException {
    if (cache.get(CACHE_KEY) == null) {
      var metadata = fetchMetadata();
      cache.put(CACHE_KEY, metadata, System.currentTimeMillis() + expirationTimeInMilliseconds);
      return metadata;
    }

    return cache.get(CACHE_KEY);
  }

  private OIDCProviderMetadata fetchMetadata() throws HelseIdException{
    try {
      return OIDCProviderMetadata.resolve(Issuer.parse(authority.toString()));
    } catch (GeneralException | IOException e) {
      throw new HelseIdException("Error occurred during fetching metadata", e);
    }
  }

  public static synchronized RemoteMetadataProvider getInstance(URI authority) {
    if (!instanceMap.containsKey(authority.toString())) {
      instanceMap.put(authority.toString(), new RemoteMetadataProvider(authority, MILLISECONDS_IN_A_DAY, new InMemoryExpiringCache<>()));
    }
    return instanceMap.get(authority.toString());
  }
}
