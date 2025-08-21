package no.helseid.clientassertion;

import java.util.Map;

abstract class ClaimsUtil {
  static Map<String, Object> createClaimSFMJournalId(String sfmJournalId) {
    return Map.of(
        "type", "nhn:sfm:journal-id",
        "value", Map.of(
            "journal_id", sfmJournalId
        )
    );
  }

  static Map<String, Object> createClaimHelseidAuthorizationForMultiTenant(String parentOrganization, String childOrganization) {
    StringBuilder organizationValue = new StringBuilder("NO:ORGNR:");
    organizationValue.append(parentOrganization);
    if (childOrganization != null) {
      organizationValue.append(":");
      organizationValue.append(childOrganization);
    }

    return Map.of(
        "type", "helseid_authorization",
        "practitioner_role", Map.of(
            "organization", Map.of(
                "identifier", Map.of(
                    "system", "urn:oid:1.0.6523",
                    "type", "ENH",
                    "value", organizationValue.toString()
                )
            )
        )
    );
  }

  static Map<String, Object> createClaimHelseidAuthorizationForSingleTenant(String childOrganization) {
    return Map.of(
        "type", "helseid_authorization",
        "practitioner_role", Map.of(
            "organization", Map.of(
                "identifier", Map.of(
                    "system", "urn:oid:2.16.578.1.12.4.1.4.101",
                    "type", "ENH",
                    "value", childOrganization
                )
            )
        )
    );
  }
}
