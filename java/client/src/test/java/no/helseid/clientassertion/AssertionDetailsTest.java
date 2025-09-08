package no.helseid.clientassertion;

import no.helseid.configuration.Tenancy;
import no.helseid.exceptions.HelseIdException;
import org.junit.jupiter.api.Test;

import java.util.List;
import java.util.Map;

import static org.junit.jupiter.api.Assertions.*;

class AssertionDetailsTest {

  @Test
  void builder_for_single_tenant_should_return_map_when_only_sfm_journal_is_set() throws HelseIdException {
    AssertionDetails assertionDetails = new AssertionDetails.Builder(Tenancy.SINGLE_TENANT)
        .withSfmJournalId("sfm-journal-id")
        .build();

    assertInstanceOf(Map.class, assertionDetails.value());
  }


  @Test
  void builder_for_single_tenant_should_return_map_when_only_organization_is_set() throws HelseIdException {
    AssertionDetails assertionDetails = new AssertionDetails.Builder(Tenancy.SINGLE_TENANT)
        .withChildOrganizationNumber("child-organization-number")
        .build();

    assertInstanceOf(Map.class, assertionDetails.value());
  }

  @Test
  void builder_for_single_tenant_should_return_list_when_organization_and_sfm_journal_is_set() throws HelseIdException {
    AssertionDetails assertionDetails = new AssertionDetails.Builder(Tenancy.SINGLE_TENANT)
        .withChildOrganizationNumber("child-organization-number")
        .withSfmJournalId("sfm-journal-id")
        .build();

    assertInstanceOf(List.class, assertionDetails.value());
  }

  @Test
  void builder_for_single_tenant_should_throw_exception_when_input_results_in_empty_assertion_details() {
    try {
      new AssertionDetails.Builder(Tenancy.SINGLE_TENANT)
          .withParentOrganizationNumber("parent-organization-number")
          .build();
      fail();
    } catch (HelseIdException e) {
      assertEquals("The resulting assertion details is empty", e.getMessage());
    }
  }

  @Test
  void builder_for_multi_tenant_should_throw_exception_when_input_results_in_empty_assertion_details() {
    try {
      new AssertionDetails.Builder(Tenancy.MULTI_TENANT)
          .withChildOrganizationNumber("child-organization-number")
          .build();
      fail();
    } catch (HelseIdException e) {
      assertEquals("The resulting assertion details is empty", e.getMessage());
    }
  }

  @Test
  void builder_for_multi_tenant_should_return_map_when_only_sfm_journal_is_set() throws HelseIdException {
    AssertionDetails assertionDetails = new AssertionDetails.Builder(Tenancy.MULTI_TENANT)
        .withSfmJournalId("sfm-journal-id")
        .build();

    assertInstanceOf(Map.class, assertionDetails.value());
  }

  @Test
  void builder_for_multi_tenant_should_return_map_when_only_organization_is_set() throws HelseIdException {
    AssertionDetails assertionDetails = new AssertionDetails.Builder(Tenancy.MULTI_TENANT)
        .withParentOrganizationNumber("parent-organization-number")
        .withChildOrganizationNumber("child-organization-number")
        .build();

    assertInstanceOf(Map.class, assertionDetails.value());
  }

  @Test
  void builder_for_multi_tenant_should_return_list_when_organization_and_journal_is_set() throws HelseIdException {
    AssertionDetails assertionDetails = new AssertionDetails.Builder(Tenancy.MULTI_TENANT)
        .withParentOrganizationNumber("parent-organization-number")
        .withChildOrganizationNumber("child-organization-number")
        .withSfmJournalId("sfm-journal-id")
        .build();

    assertInstanceOf(List.class, assertionDetails.value());
  }

}