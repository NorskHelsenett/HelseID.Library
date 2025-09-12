package no.helseid.testutil;

import com.github.tomakehurst.wiremock.http.Request;
import com.github.tomakehurst.wiremock.matching.MatchResult;
import com.nimbusds.jwt.SignedJWT;

import java.text.ParseException;

public class HelseIdRequestMatcher {
  private static final String DPOP = "DPoP";
  private static final String NONCE = "nonce";

  public static MatchResult hasDPoPProofContainingNonce(Request request, String expectedNonce) {
    try {
      SignedJWT dPoPProof = SignedJWT.parse(request.getHeader(DPOP));
      String actualNonce = dPoPProof.getJWTClaimsSet().getStringClaim(NONCE);

      if (expectedNonce == null) {
        return MatchResult.of(actualNonce == null);
      }

      return MatchResult.of(expectedNonce.equals(actualNonce));
    } catch (ParseException e) {
     return MatchResult.noMatch();
    }
  }
}
