package no.helseid.clientassertion;

import java.util.Map;

/**
 * Util for generating claim of type nhn:sfm:journal-id
 */
interface SFMJournalIdClaimUtil {
  String CLAIM_TYPE = "nhn:sfm:journal-id";

  /**
   * Create claim of type nhn:sfm:journal-id specifying the journal_id in SFM (Sentral Forskrivningsmodul).
   *
   * @param sfmJournalId the journal id in SFM
   * @return A map representation of a JSON claim of type nhn:sfm:journal-id
   */
  static Map<String, Object> createClaimSFMJournalId(String sfmJournalId) {
    return Map.of(
        "type", CLAIM_TYPE,
        "value", Map.of(
            "journal_id", sfmJournalId
        )
    );
  }
}
