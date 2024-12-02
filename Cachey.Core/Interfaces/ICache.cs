namespace Cachey.Core;

/// <summary>
/// Interface for caching, which can be implemented in different caching types.
/// </summary>
public interface ICache
{
    /// <summary>
    /// Retrieves a cached value by its key. If the value is not found or has expired, it returns
    /// default(T).
    /// </summary>
    /// <typeparam name="T">The type expected to be returned.</typeparam>
    /// <param name="key">The key of the cached value.</param>
    /// <returns>
    ///  Cached value if found and not expired, otherwise
    ///  default(T).
    /// </returns>
    T? Get<T>(string key);
    /// <summary>
    /// Asynchronously retrieves a cached value by its key. If the value is not found or has expired, it returns
    /// default(T).
    /// </summary>
    /// <typeparam name="T">The type expected to be returned.</typeparam>
    /// <param name="key">The key of the cached value.</param>
    /// <param name="expiration">Optional expiration time.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    ///  A task that represents the asynchronous operation, with the cached value if found and not expired, otherwise
    ///  default(T).
    /// </returns>
    Task<T?> GetAsync<T>(string key, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    /// <summary>
    /// Adds a new item to the cache. If no expiration time is specified, the value will remain in the cache until removed.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The key under which the value will be cached.</param>
    /// <param name="value">The value to be cached.</param>
    /// <param name="expiration">Optional expiration time.</param>
    void Set<T>(string key, T value, TimeSpan? expiration = null);
    /// <summary>
    /// Adds a new item to the cache asynchronously. If no expiration time is specified, the value will remain in the cache
    /// until removed.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The key under which the value will be cached.</param>
    /// <param name="value">The value to be cached.</param>
    /// <param name="expiration">Optional expiration time.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration, CancellationToken cancellationToken = default);
    /// <summary>
    /// Removes a cached value by its key.
    /// </summary>
    /// <param name="key">The key of the cached value to be removed.</param>
    void Remove(string key);
    /// <summary>
    /// Removes a cached value by its key asynchronously.
    /// </summary>
    /// <param name="key">The key of the cached value to be removed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    /// <summary>
    /// Check if key exists in cache.
    /// </summary>
    /// <param name="key">The key of the cached value to be searched.</param>
    /// <returns>Return a value that represents existence of the given key.</returns>
    bool Contains(string key);
    /// <summary>
    /// Check if key exists in cache asynchronously.
    /// </summary>
    /// <param name="key">The key of the cached value to be searched.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Return a value that represents existence of the given key.</returns>
    Task<bool> ContainsAsync(string key, CancellationToken cancellationToken = default);
    /// <summary>
    /// Clears all cached items.
    /// </summary>
    void Clear();
    /// <summary>
    /// Asynchronously clears all cached items.
    /// </summary>
    Task ClearAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Removes expired items from cache
    /// </summary>
    Task RemoveExpiredItemsAsync();
}