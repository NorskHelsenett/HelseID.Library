package no.helseid.cache;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;

class HelseIdInMemoryCacheTest {
  @Test
  void cache_should_store_value() {
    var cache = new HelseIdInMemoryCache<String>(1000);
    cache.put("foo", "bar");
    assertEquals("bar", cache.get("foo"));
  }

  @Test
  void cache_should_return_null_after_expiry() {
    var cache = new HelseIdInMemoryCache<String>(-1);
    cache.put("foo", "bar");
    assertNull(cache.get("foo"));
  }

  @Test
  void cache_should_store_value_until_expiry() throws InterruptedException {
    var cache = new HelseIdInMemoryCache<String>(1);
    cache.put("foo", "bar");
    assertEquals("bar", cache.get("foo"));
    Thread.sleep(2);
    assertNull(cache.get("foo"));
  }

  @Test
  void cache_should_remove_value_on_request() {
    var cache = new HelseIdInMemoryCache<String>(100);
    cache.put("foo", "bar");
    assertEquals("bar", cache.get("foo"));
    var removed = cache.remove("foo");
    assertEquals("bar", removed);
    assertNull(cache.get("foo"));
  }


  @Test
  void cache_should_remove_all_values_on_clear() {
    var cache = new HelseIdInMemoryCache<String>(100);
    cache.put("foo", "bar");
    cache.put("bar", "foo");
    assertEquals("bar", cache.get("foo"));
    assertEquals("foo", cache.get("bar"));
    cache.clear();
    assertNull(cache.get("foo"));
    assertNull(cache.get("bar"));
  }
}