package no.helseid.exceptions;

public class HelseIdException extends Exception {
  public HelseIdException(String message) {
    super(message);
  }

  public HelseIdException(String message, Throwable cause) {
    super(message, cause);
  }
}
