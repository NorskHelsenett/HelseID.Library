package no.helseid.cache;

import java.util.concurrent.ConcurrentHashMap;

/**
 * An in-memory implementation of an expiring cache
 * @param <T> the class of the cached values
 */
public class InMemoryExpiringCache<T> implements ExpiringCache<T> {
  private final ConcurrentHashMap<String, ExpiringValue<T>> cache = new ConcurrentHashMap<>();

  @Override
  public T get(String key) {
    var expiringValue = cache.get(key);

    if (expiringValue == null ||expiringValue.expireAtEpochMilliseconds < System.currentTimeMillis()) {
      remove(key);
      return null;
    }

    return expiringValue.value;
  }

  @Override
  public void put(String key, T value, long expireAtEpochMilliseconds) {
    cache.put(key, new ExpiringValue<>(value, expireAtEpochMilliseconds));
  }

  @Override
  public void remove(String key) {
    cache.remove(key);
  }

  /**
   * Internal class representing the expiration of a value
   * @param value
   * @param expireAtEpochMilliseconds
   * @param <T>
   */
  private record ExpiringValue<T>(T value, long expireAtEpochMilliseconds) {
  }
}
