package no.helseid.metadata;

import com.nimbusds.oauth2.sdk.GeneralException;
import com.nimbusds.oauth2.sdk.id.Issuer;
import com.nimbusds.openid.connect.sdk.op.OIDCProviderMetadata;
import no.helseid.cache.HelseIdInMemoryCache;
import no.helseid.exceptions.HelseIdConfigurationException;
import no.helseid.exceptions.HelseIdException;

import java.io.IOException;
import java.net.URI;
import java.util.concurrent.TimeUnit;

public class RemoteHelseIdMetadataProvider implements HelseIdMetadataProvider {
  private static final long MILLISECONDS_IN_A_DAY = TimeUnit.DAYS.toMillis(1);
  private static final String CACHE_KEY = "discovery";
  private final HelseIdInMemoryCache<OIDCProviderMetadata> cache;
  private final URI stsUri;

  public RemoteHelseIdMetadataProvider(URI stsUri) {
    this(stsUri, MILLISECONDS_IN_A_DAY);
  }

  RemoteHelseIdMetadataProvider(URI stsUri, Long cacheTimeInMilliseconds) {
    this.stsUri = stsUri;
    this.cache = new HelseIdInMemoryCache<>(cacheTimeInMilliseconds);
  }

  @Override
  public OIDCProviderMetadata getMetadata() throws HelseIdException {
    var metadata = cache.get(CACHE_KEY);

    if (metadata == null) {
      try {
        metadata = OIDCProviderMetadata.resolve(Issuer.parse(stsUri.toString()));
      } catch (GeneralException | IOException e) {
        throw new HelseIdConfigurationException("Error occurred during fetching metadata", e);
      }
      cache.put(CACHE_KEY, metadata);
    }

    return metadata;
  }
}
