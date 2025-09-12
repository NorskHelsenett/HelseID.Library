package no.helseid.signing;

import no.helseid.exceptions.HelseIdException;
import org.junit.jupiter.api.Test;

import java.security.ProviderException;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.fail;

class ECKeyReferenceTest {

  @Test
  public void should_reject_algorithms_not_compatible_with_EC() {
    try {
      ECKeyReference.generate(Algorithm.RS256);
      fail();
    } catch (HelseIdException e) {
      assertEquals("Algorithm must be compatible with elliptic curve", e.getMessage());
    }
  }

  @Test
  public void should_generate_with_ES256() throws HelseIdException {
    ECKeyReference.generate(Algorithm.ES256);
  }

  @Test
  public void should_generate_with_ES384() throws HelseIdException {
    ECKeyReference.generate(Algorithm.ES384);
  }

  @Test
  public void should_generate_with_ES512() throws HelseIdException {
    try {
      ECKeyReference.generate(Algorithm.ES512);
    }  catch (ProviderException e) {
      // Catching if the algorithm is not provided
    }
  }
}