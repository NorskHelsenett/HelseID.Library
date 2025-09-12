package no.helseid.signing;

import no.helseid.exceptions.HelseIdException;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.fail;

class RSAKeyReferenceTest {

  @Test
  public void should_reject_algorithms_not_compatible_with_RSA() {
    try {
      RSAKeyReference.generate(Algorithm.ES256);
      fail();
    } catch (HelseIdException e) {
      assertEquals("Algorithm must be RSA compatible", e.getMessage());
    }
  }

  @Test
  public void should_generate_with_RS256() throws HelseIdException {
    RSAKeyReference.generate(Algorithm.RS256);
  }

  @Test
  public void should_generate_with_RS384() throws HelseIdException {
    RSAKeyReference.generate(Algorithm.RS384);
  }

  @Test
  public void should_generate_with_RS512() throws HelseIdException {
    RSAKeyReference.generate(Algorithm.RS512);
  }

  @Test
  public void should_generate_with_PS256() throws HelseIdException {
    RSAKeyReference.generate(Algorithm.PS256);
  }

  @Test
  public void should_generate_with_PS384() throws HelseIdException {
    RSAKeyReference.generate(Algorithm.PS384);
  }

  @Test
  public void should_generate_with_PS512() throws HelseIdException {
    RSAKeyReference.generate(Algorithm.PS512);
  }
}