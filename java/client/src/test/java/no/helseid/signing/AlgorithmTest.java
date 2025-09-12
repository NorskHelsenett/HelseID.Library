package no.helseid.signing;

import no.helseid.exceptions.HelseIdException;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

class AlgorithmTest {

  @Test
  void algortithm_should_parse_RS256() throws HelseIdException {
    assertEquals(Algorithm.RS256, Algorithm.parse("RS256"));
  }

  @Test
  void algortithm_should_parse_RS384() throws HelseIdException {
    assertEquals(Algorithm.RS384, Algorithm.parse("RS384"));
  }

  @Test
  void algortithm_should_parse_RS512() throws HelseIdException {
    assertEquals(Algorithm.RS512, Algorithm.parse("RS512"));
  }

  @Test
  void algortithm_should_parse_PS256() throws HelseIdException {
    assertEquals(Algorithm.PS256, Algorithm.parse("PS256"));
  }

  @Test
  void algortithm_should_parse_PS384() throws HelseIdException {
    assertEquals(Algorithm.PS384, Algorithm.parse("PS384"));
  }

  @Test
  void algortithm_should_parse_PS512() throws HelseIdException {
    assertEquals(Algorithm.PS512, Algorithm.parse("PS512"));
  }

  @Test
  void algortithm_should_parse_ES256() throws HelseIdException {
    assertEquals(Algorithm.ES256, Algorithm.parse("ES256"));
  }

  @Test
  void algortithm_should_parse_ES384() throws HelseIdException {
    assertEquals(Algorithm.ES384, Algorithm.parse("ES384"));
  }

  @Test
  void algortithm_should_parse_ES512() throws HelseIdException {
    assertEquals(Algorithm.ES512, Algorithm.parse("ES512"));
  }
}