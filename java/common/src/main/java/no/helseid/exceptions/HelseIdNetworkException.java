package no.helseid.exceptions;

public class HelseIdNetworkException extends HelseIdException {
  public final boolean requestWasSent;
  public final Integer httpStatus;

  public HelseIdNetworkException(String message, Throwable cause, boolean requestWasSent, Integer httpStatus) {
    super(message, cause);
    this.requestWasSent = requestWasSent;
    this.httpStatus = httpStatus;
  }
}
