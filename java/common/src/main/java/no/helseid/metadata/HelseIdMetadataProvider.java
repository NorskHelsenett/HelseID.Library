package no.helseid.metadata;

import com.nimbusds.openid.connect.sdk.op.OIDCProviderMetadata;
import no.helseid.exceptions.HelseIdException;

public interface HelseIdMetadataProvider {
  OIDCProviderMetadata getMetadata() throws HelseIdException;
}
