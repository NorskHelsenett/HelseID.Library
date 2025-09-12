package no.helseid.metadata;

import com.nimbusds.openid.connect.sdk.op.OIDCProviderMetadata;
import no.helseid.exceptions.HelseIdException;

/**
 * Interface for providing metadata about HelseID STS
 */
public interface MetadataProvider {
  OIDCProviderMetadata getMetadata() throws HelseIdException;
}
