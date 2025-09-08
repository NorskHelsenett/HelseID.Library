package no.helseid.clientassertion;

import org.junit.jupiter.api.Test;

import java.util.Map;
import java.util.UUID;

import static no.helseid.clientassertion.HelseIdAuthorizationClaimUtil.*;
import static org.junit.jupiter.api.Assertions.assertEquals;

class HelseIdAuthorizationClaimUtilTest {

  @Test
  void should_create_claim_map_for_multi_tenant_with_parent_and_child_organization() {
    String parentOrganizationNumber = UUID.randomUUID().toString();
    String childOrganizationNumber = UUID.randomUUID().toString();

    Map<String, Object> map = HelseIdAuthorizationClaimUtil.createClaimHelseidAuthorizationForMultiTenant(parentOrganizationNumber, childOrganizationNumber);
    assertEquals(CLAIM_TYPE, map.get("type"));

    Map<String, Object> practitionerRoleMap = (Map<String, Object>) map.get("practitioner_role");
    Map<String, Object> organizationMap = (Map<String, Object>) practitionerRoleMap.get("organization");
    Map<String, Object> identifierMap = (Map<String, Object>) organizationMap.get("identifier");

    assertEquals(MULIT_TENANT_ORGANIZATION_IDENTIFIER_SYSTEM, identifierMap.get("system"));
    assertEquals(ORGANIZATION_IDENTIFIER_TYPE, identifierMap.get("type"));
    assertEquals(MULTI_TENANT_ORGANIZATION_VALUE_PREFIX + parentOrganizationNumber + ":"  + childOrganizationNumber, identifierMap.get("value"));
  }

  @Test
  void should_create_claim_map_for_multi_tenant_with_only_parent_organization() {
    String parentOrganizationNumber = UUID.randomUUID().toString();

    Map<String, Object> map = HelseIdAuthorizationClaimUtil.createClaimHelseidAuthorizationForMultiTenant(parentOrganizationNumber, null);
    assertEquals(CLAIM_TYPE, map.get("type"));

    Map<String, Object> practitionerRoleMap = (Map<String, Object>) map.get("practitioner_role");
    Map<String, Object> organizationMap = (Map<String, Object>) practitionerRoleMap.get("organization");
    Map<String, Object> identifierMap = (Map<String, Object>) organizationMap.get("identifier");

    assertEquals(MULIT_TENANT_ORGANIZATION_IDENTIFIER_SYSTEM, identifierMap.get("system"));
    assertEquals(ORGANIZATION_IDENTIFIER_TYPE, identifierMap.get("type"));
    assertEquals(MULTI_TENANT_ORGANIZATION_VALUE_PREFIX + parentOrganizationNumber, identifierMap.get("value"));
  }

  @Test
  void should_create_claim_map_for_single_tenant_with_child_organization() {
    String childOrganizationNumber = UUID.randomUUID().toString();

    Map<String, Object> map = HelseIdAuthorizationClaimUtil.createClaimHelseidAuthorizationForSingleTenant(childOrganizationNumber);
    assertEquals(CLAIM_TYPE, map.get("type"));

    Map<String, Object> practitionerRoleMap = (Map<String, Object>) map.get("practitioner_role");
    Map<String, Object> organizationMap = (Map<String, Object>) practitionerRoleMap.get("organization");
    Map<String, Object> identifierMap = (Map<String, Object>) organizationMap.get("identifier");

    assertEquals(SINGLE_TENANT_ORGANIZATION_IDENTIFIER_SYSTEM, identifierMap.get("system"));
    assertEquals(ORGANIZATION_IDENTIFIER_TYPE, identifierMap.get("type"));
    assertEquals(childOrganizationNumber, identifierMap.get("value"));
  }

}