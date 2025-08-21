package no.helseid.cache;

public interface HelseIdCache<T> {
  void put(String key, T value);
  T get(String key);
  T remove(String key);
  void clear();
}
