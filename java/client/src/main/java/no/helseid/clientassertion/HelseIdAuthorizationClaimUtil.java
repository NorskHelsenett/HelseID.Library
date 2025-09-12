package no.helseid.clientassertion;

import java.util.Map;

/**
 * Util for generating claim of type helseid_authorization
 */
interface HelseIdAuthorizationClaimUtil {
  /**
   * Claim type of a helseid authorization
   */
  String CLAIM_TYPE = "helseid_authorization";
  /**
   * The identifier type of organization
   */
  String ORGANIZATION_IDENTIFIER_TYPE = "ENH";
  /**
   * The system for identifying single tenant organizations
   */
  String SINGLE_TENANT_ORGANIZATION_IDENTIFIER_SYSTEM = "urn:oid:2.16.578.1.12.4.1.4.101";
  /**
   * The system for identifying multi tenant organizations
   */
  String MULIT_TENANT_ORGANIZATION_IDENTIFIER_SYSTEM = "urn:oid:1.0.6523";
  /**
   * Prefix in identifying multi tenant organizations
   */
  String MULTI_TENANT_ORGANIZATION_VALUE_PREFIX = "NO:ORGNR:";

  /**
   * Create claim of type helseid_authorization with specification for a multi-tenant client.
   *
   * @param parentOrganization A multi-tenant client may represent multiple parent organizations thus a specification of the current organization is required.
   * @param childOrganization  A specification of child organization is optional but recommended the child organization is different from the parent organization.
   * @return A map representation of a JSON claim of type helseid_authorization
   */
  static Map<String, Object> createClaimHelseidAuthorizationForMultiTenant(String parentOrganization, String childOrganization) {
    StringBuilder organizationValue = new StringBuilder(MULTI_TENANT_ORGANIZATION_VALUE_PREFIX);
    organizationValue.append(parentOrganization);
    if (childOrganization != null) {
      organizationValue.append(":");
      organizationValue.append(childOrganization);
    }

    return Map.of(
        "type", CLAIM_TYPE,
        "practitioner_role", Map.of(
            "organization", Map.of(
                "identifier", Map.of(
                    "system", MULIT_TENANT_ORGANIZATION_IDENTIFIER_SYSTEM,
                    "type", ORGANIZATION_IDENTIFIER_TYPE,
                    "value", organizationValue.toString()
                )
            )
        )
    );
  }

  /**
   * Create claim of type helseid_authorization with specification for a single-tenant client.
   * A single-tenant is bound to one parent organization, the helseid_authorization itself is optional and only necessary if child organization should be specified.
   *
   * @param childOrganization A specification of child organization is required in a helseid_authorization for single-tenants.
   * @return A map representation of a JSON claim of type helseid_authorization
   */
  static Map<String, Object> createClaimHelseidAuthorizationForSingleTenant(String childOrganization) {
    return Map.of(
        "type", CLAIM_TYPE,
        "practitioner_role", Map.of(
            "organization", Map.of(
                "identifier", Map.of(
                    "system", SINGLE_TENANT_ORGANIZATION_IDENTIFIER_SYSTEM,
                    "type", ORGANIZATION_IDENTIFIER_TYPE,
                    "value", childOrganization
                )
            )
        )
    );
  }
}
