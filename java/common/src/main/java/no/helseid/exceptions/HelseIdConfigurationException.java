package no.helseid.exceptions;

public class HelseIdConfigurationException extends HelseIdException {
  public HelseIdConfigurationException(String message) {
    super(message);
  }

  public HelseIdConfigurationException(String message, Throwable cause) {
    super(message, cause);
  }
}
