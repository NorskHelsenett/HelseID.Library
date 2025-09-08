package no.helseid.cache;

import org.junit.jupiter.api.Test;

import java.util.UUID;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;

class HelseIdInMemoryExpiringCacheTest {
  @Test
  public void values_stored_in_cache_should_be_accessible() {
    InMemoryExpiringCache<String> memoryCache = new InMemoryExpiringCache<>();
    var key = UUID.randomUUID().toString();
    var value = UUID.randomUUID().toString();

    memoryCache.put(key, value, System.currentTimeMillis() + 100);
    assertEquals(value, memoryCache.get(key));
  }

  @Test
  public void values_stored_in_cache_should_expire_at_expiry_timestamp() {
    InMemoryExpiringCache<String> memoryCache = new InMemoryExpiringCache<>();
    var key = UUID.randomUUID().toString();
    var value = UUID.randomUUID().toString();

    memoryCache.put(key, value, System.currentTimeMillis() - 1);

    assertNull(memoryCache.get(key));
  }

  @Test
  public void values_stored_in_cache_should_be_removed_upon_request() {
    InMemoryExpiringCache<String> memoryCache = new InMemoryExpiringCache<>();
    var key = UUID.randomUUID().toString();
    var value = UUID.randomUUID().toString();

    memoryCache.put(key, value, System.currentTimeMillis() + 100);
    assertEquals(value, memoryCache.get(key));
    memoryCache.remove(key);
    assertNull(memoryCache.get(key));
  }
}