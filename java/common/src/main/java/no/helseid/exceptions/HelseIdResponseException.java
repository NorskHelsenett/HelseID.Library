package no.helseid.exceptions;

import com.nimbusds.oauth2.sdk.ErrorObject;

public class HelseIdResponseException extends HelseIdException {
  public final int statusCode;
  public final ErrorObject errorObject;

  public HelseIdResponseException(String message, int statusCode, ErrorObject errorObject) {
    super(message);
    this.statusCode = statusCode;
    this.errorObject = errorObject;
  }
}
