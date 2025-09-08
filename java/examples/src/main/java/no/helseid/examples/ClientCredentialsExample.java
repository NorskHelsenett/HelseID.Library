package no.helseid.examples;

import no.helseid.clientassertion.AssertionDetails;
import no.helseid.configuration.Client;
import no.helseid.configuration.Tenancy;
import no.helseid.dpop.DPoPProofCreator;
import no.helseid.dpop.DefaultDPoPProofCreator;
import no.helseid.dpop.HttpMethod;
import no.helseid.endpoints.token.AccessTokenResponse;
import no.helseid.endpoints.token.ErrorResponse;
import no.helseid.endpoints.token.TokenResponse;
import no.helseid.exceptions.HelseIdException;
import no.helseid.grants.ClientCredentials;
import no.helseid.signing.JWKKeyReference;

import java.net.URI;
import java.util.Collections;
import java.util.List;

public class ClientCredentialsExample {
  public static final String JWK = "{\"alg\":\"PS256\",\"d\":\"AQ9xGpUrOEFK2VPUBsuZpFobYuZiGEcQnC5uxKNOxGuXzhJIec9_MGVbiJUfPVb1J4mtPyIcIKAQr-hzoj4ZhzVVpbAo4hj_tepl2CzlpXIsB2eqylyBrDYCkL3GnwYFQUAWBsoJ6gSmyUJSKkki3DNFqG0rH8h7Y9JCO15ulSE--qgoGcJN-v5LzZ9IRDTpWJVtEO0m6nirBLM60ONobR94EPpFgsKoNr0AyLtCnEiZU-h4uFy7Qlpk9QKa0iM8zEhdOIFXmcr0dUBwCR7BR7ApAUo8Asl8jY9fl8G2jgmieiunvOUsdSsUo161-vm_dqV3AtkAFkPzXpQfW2Te3POrPAbyhrc_OgFlanmdB7brt5MiJ2q62qAArIUE3sXfn3anKJ2crQYBBlESm-hxIY5xSfafjdld3u3USShf87kCE0jGbdqc7a23qZaYLJw0aRxtgjIVizormSJeo-ZUQMfrqhUtxvFmQGneUFXTxugftmB84QOcbCOgojAOQ422dm2UG56Bbv2KNyFeB9Etihsb_H53VWfwdDL4rU-4J_8IHsbFkxzh4auk7c7QltK5R_LblCy7VUc24hTk_MlZazNqXP2hpC8oJyidolgYjgjm0S0Mo9hAe_BS5VaLx5MV0uanL0ezUns-56mFS1ygaRUckq1k-0jdYCBWCoADgME\",\"dp\":\"UAE-sFxlzJZdPtueN7kq_hLTaT2F4TUSHoFF4e-B7Xkwq24J0jlwktf0u5W_QpAETtG1NwwxNAauK03tjINiUkALyxCmMIIavS63eBNA0XVcRqohQ9lyNJ2N-Cu9xx0FJZtm-YO1H6hfXr1fEplpe_A4Y2cu3uiwoydP8-OYBgF-AW4DThhq26-DhfIaDVFhba9iK8NgWFP3SkEs2IZ6toZkwbSWm9lKFOuq1XJJD643wQOB2PRGJNbQ8EqLPlu0MlCt0BF0SIquP8EemprT8tIuJZ--KKn7OWqCWl6l40TnKUz2rqPvOoWTo-VlEj-iNK2at6MZT0C1jw26dFTjcQ\",\"dq\":\"igQ4F1p0wQNjsv5eGDGEcoLDksXrHqX_EGc5xFp3L4PaqGpr29LihkV1YF5KWSLgOIARAshaBfUqibO_GFbPniCZZy9LxT7HzIle-oM_z45YyBI_hVmiMZakOxKiIALZP_ftCIOProYqPdqJADRSqfH4iZoG0eWGAdrTs7xURd8uDmbiBO9luyo_JUA4QSKaaaS77GU9q_aiEEJecBUVGxz2kw0ghu_gTDL9gTTNgyrBCKQ2eOmLClu8b1doRxIWaDzBNBLjOR7JoYNgm88ueEWaE8Z5eM0ENNQwvNY9-8yTZfVAMagGSIxY9icC_XpzSd_hXFzL6QXSEV2hzypMjw\",\"e\":\"AQAB\",\"key_ops\":[\"sign\"],\"kty\":\"RSA\",\"n\":\"rt1br92h922jmIt44tRH9ovWP6-C06c-QV1gQ91-cFKGNM2E_8mEKsCCyr09hAMDIVS4LS_RqXNHPB3sayfK4uI3H2v37Yj-KyIp3wS58efikBq2snBOs-_AQm98AW2xXiLAEOiFK7OG31UDtZqah_3C5l_BIVyVQjk7LrFWbjkSZ4em9-fwNEKwkxGTSh6EJSh02KE4kFguF5_M7kNL93_ZPtdl-rhmQKgy23IqQxykEd6EHAfMf8vdGmIzE7cCEo1JS7bTNTid3SBqGvDgCVxkGfsPTILpt6V2PsMjfCjHiVucuAH2957u9JwaEsyrQk5fEvFgKBTi-y7PKLYkPY00D2HfdJR56yORHBXaaDTSJfW0xRbLg0HPSzXRHW1bnwCxEcQBUqCNDZDWB69K0vNs3VlM18y2BueW_qzv-N4wb-i5UW-anazXDXFcesuGWeWX--LbjggGAnyRNPmP9mB6JYGlplqRBMK4EAtx2tE2JnJQwA_ndZYkdUugnXr8F65Sl1VnUgjpX8ZVAJyyn_U5YbSCNqIScUd_PxMhG7lx4pYf2MrjBov5XMRvtI-iHJjlhc6aj14HV2VaH4FKBQ3UzbIZi-D8LI7Notgc7DqjrZ4XKnJHPGUO-qSgBdEFoNS4U1xXn4D1j0G8MMO9hCgKAemXeRlFk1t7JBYtk58\",\"p\":\"86h5XO-rBfxhqSRtiOi6JZc1tpwke7XyyjV3oiEwg3gey6DX2kFwpqbnAASBxTL2gTIexfM55xZhK43aCB_eYFFxlri4K8hQytQGylf5G8fGLvjZcX0juc7ZzBSUjKZEoFLCO4m7oqeDsffQINcKnaX-yDZevpmT9FdDBu4kci0u0tRQIsguZb40AUGuK1H2r5PiGx5ZYvJTkDny6QsvfaC_lPKk3cvkhYv1fyyzeDqeeb1Xq6fQC_SZvUEy8IKbIU8BhQ2gKXuHx45AEjHgZhjT2ZK3bK0FVZeFwyxjhlst-3UAY0uToO7b74ABbF-OzAWn0Hu8Bx1DV9K3WVi1kQ\",\"q\":\"t7jWKmmA940WwuiHWlAMasyPJajEqHUWOH3jHkoyv-XDTKJxK0AUGRASFXkp1ur2rxkuDwjOsTbS3YhA6g_L8eDp0mbV49H00SP-1at-v5oLdmNtgpkcIcLe4ZdQbZhlY6Yx44oh_HFL5AItjcSfimJ3GoQ7cXyUpcsrB-4Hl7tCPNTsEX_W9LtkBncXw8fhvRhryV3vILlEivCgv3Psrb7LF6xIFwF_rjw2NA_HomfXIG_6PphHsZ9vIThxA6BKWRFOnpqLMp6ou-IUyNBV0GbXvtGRQGwjuLlD7guMOOYTM0c7fB1DkuwxCVAJF3wDXwLPIPD6s1P8sE_iiCleLw\",\"qi\":\"nWNaYD__BINED1Lj4UdeWGfEfU-vcsMLzQfkFoGLIJGTP5a_FSy2yQhZpRZ1eUceLKcsgKvPD-1z3w9bRrKchjPnL1UUVlNqLjnO6Ky2OSqybid_dv6UittItU-AUDviarMs4T0ko4VLZVw9a_YhKXLdBtm8DiG9CGN0XuylGRyqn7mLieYwtbh5PBAyQoxTKBeWBVRvobmkRADiF9ecwIbd9DCv7xewylAS5u_JxFomFv8vd6arBgMLoylO9TxoWA2jP7vMhCzjz2OQiUmJNTYEnBxizSyjdQYhBFEVT9vrA43VQj4HsNounSF8VreToir6Yir9viut9O7IZaZDzA\",\"kid\":\"fumDvzq49GP4xOxmD_E3MLhpbgM57HYGYX9BObra-3k\"}";
  private static final String CLIENT_ID = "3adfb881-51d4-4e49-b37f-58ce96e9d006";
  private static final List<String> SCOPE = Collections.singletonList("nhn:selvbetjening/client");
  private static final URI AUTHORITY = URI.create("https://helseid-sts.test.nhn.no");
  private static final Tenancy TENANCY = Tenancy.MULTI_TENANT;

  public static void main(String[] args) throws HelseIdException {
    Client client = new Client(CLIENT_ID, JWKKeyReference.parse(JWK), SCOPE);

    DPoPProofCreator dPoPProofCreator = new DefaultDPoPProofCreator(client.keyReference());

    ClientCredentials clientCredentials = new ClientCredentials.Builder(AUTHORITY)
        .withClient(client)
        .setCustomDPoPProofCreator(dPoPProofCreator)
        .build();


    AssertionDetails assertionDetails = new AssertionDetails.Builder(TENANCY)
        .withParentOrganizationNumber("994598759")
        .withChildOrganizationNumber("994598759")
        .build();
    TokenResponse tokenResponse = clientCredentials.getAccessToken(assertionDetails);

    if (tokenResponse instanceof ErrorResponse errorResponse) {
      handleErrorResponse(errorResponse);
    }

    if (tokenResponse instanceof AccessTokenResponse accessTokenResponse) {
      String accessToken = accessTokenResponse.accessToken();

      // You can use the access token
      String dPoPProof = dPoPProofCreator.createDPoPProof(
          URI.create("https://api.no/"),
          HttpMethod.GET,
          accessToken);
    }
  }

  private static void handleErrorResponse(ErrorResponse errorResponse) {
    System.out.printf("Something went wrong: %s\n%s\n\n%s", errorResponse.error(), errorResponse.errorDescription(), errorResponse.rawResponse());
    System.exit(1);
  }
}
