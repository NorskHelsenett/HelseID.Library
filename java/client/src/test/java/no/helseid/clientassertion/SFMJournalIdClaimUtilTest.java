package no.helseid.clientassertion;

import org.junit.jupiter.api.Test;

import java.util.Map;
import java.util.UUID;

import static no.helseid.clientassertion.SFMJournalIdClaimUtil.CLAIM_TYPE;
import static org.junit.jupiter.api.Assertions.*;

class SFMJournalIdClaimUtilTest {

  @Test
  void testCreateSFMJournalIdClaim() {
    String journalId = UUID.randomUUID().toString();

    Map<String, Object> map = SFMJournalIdClaimUtil.createClaimSFMJournalId(journalId);
    assertEquals(CLAIM_TYPE, map.get("type"));

    Map<String, Object> value = (Map<String, Object>) map.get("value");
    assertEquals(journalId, value.get("journal_id"));
  }

}