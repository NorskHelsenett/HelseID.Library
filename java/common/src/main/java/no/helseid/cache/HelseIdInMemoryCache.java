package no.helseid.cache;

import java.util.Date;
import java.util.LinkedHashMap;

public class HelseIdInMemoryCache<T> implements HelseIdCache<T> {
  private final LinkedHashMap<String, InMemoryContainer<T>> cache;
  private final long lifetimeInMilliseconds;

  public HelseIdInMemoryCache(long lifetimeInMilliseconds) {
    this.lifetimeInMilliseconds = lifetimeInMilliseconds;
    this.cache = new LinkedHashMap<>();
  }

  @Override
  public T get(String key) {
    var container = cache.get(key);
    if (container == null) {
      return null;
    }
    if (container.expiryEpocMilliseconds < new Date().getTime()) {
      cache.remove(key);
      return null;
    }
    return container.value;
  }

  @Override
  public void put(String key, T value) {
    cache.put(key, new InMemoryContainer<>(value, new Date().getTime() + lifetimeInMilliseconds));
  }

  private static final class InMemoryContainer<T> {
    final T value;
    final long expiryEpocMilliseconds;

    InMemoryContainer(T value, long expiryEpocSeconds) {
      this.value = value;
      this.expiryEpocMilliseconds = expiryEpocSeconds;
    }
  }

  @Override
  public T remove(String key) {
    return cache.remove(key).value;
  }

  @Override
  public void clear() {
    cache.clear();
  }
}
