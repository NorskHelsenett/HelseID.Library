package no.helseid.configuration;

import java.net.URI;

public class HelseIdConfiguration {
  public final URI stsUri;

  public HelseIdConfiguration(URI stsUri) {
    this.stsUri = stsUri;
  }
}
