using System.Collections.Concurrent;
using Cachey.Common;
using Cachey.Common.Interfaces;

namespace Cachey.Core;

/// <summary>
/// MemoryCache is a class that provides in-memory caching functionality with support for both asynchronous and
/// synchronous operations.
/// It offers a simple way to store and retrieve data, with support for expiration times.
/// </summary>
public class MemoryCache : ICache
{
    private readonly ConcurrentDictionary<string, object> _cache = new();
    private readonly MemoryCacheMetrics _metrics = new();
    // private readonly ICache _persistentCache;

    // public MemoryCache(ICache? persistentCache = null)
    // {
    //     _persistentCache = persistentCache;
    // }

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
    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out var value))
            if (value is CacheItem<T> { IsExpired: false } cacheItem)
            {
                _metrics.RegisterHit();
                return cacheItem.Value;
            }

        _metrics.RegisterMiss();
        return default;
    }

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
    public async Task<T?> GetAsync<T>(string key, TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var value))
            if (value is CacheItem<T> { IsExpired: false } cacheItem)
            {
                _metrics.RegisterHit();
                return cacheItem.Value;
            }

        // if (_persistentCache != null) return await _persistentCache.GetAsync<T>(key);

        _metrics.RegisterMiss();
        return default;
    }

    /// <summary>
    /// Adds a new item to the cache. If no expiration time is specified, the value will remain in the cache until removed.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The key under which the value will be cached.</param>
    /// <param name="value">The value to be cached.</param>
    /// <param name="expiration">Optional expiration time.</param>
    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var cacheItem = new CacheItem<T>(value, expiration);
        _cache[key] = cacheItem;

       // if (_persistentCache != null) _persistentCache.Set<T>(key, value, expiration);
    }

    /// <summary>
    /// Adds a new item to the cache asynchronously. If no expiration time is specified, the value will remain in the cache
    /// until removed.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The key under which the value will be cached.</param>
    /// <param name="value">The value to be cached.</param>
    /// <param name="expiration">Optional expiration time.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var cacheItem = new CacheItem<T>(value, expiration);
        _cache[key] = cacheItem;
        await Task.CompletedTask;

        // if (_persistentCache != null) await _persistentCache.SetAsync<T>(key, value, expiration);
    }

    /// <summary>
    /// Removes a cached value by its key.
    /// </summary>
    /// <param name="key">The key of the cached value to be removed.</param>
    public void Remove(string key)
    {
        _cache.TryRemove(key, out _);

        // if (_persistentCache != null) _persistentCache.Remove(key);
    }

    /// <summary>
    /// Removes a cached value by its key asynchronously.
    /// </summary>
    /// <param name="key">The key of the cached value to be removed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Remove(key);

        // if (_persistentCache != null) await _persistentCache.RemoveAsync(key);
    }

    /// <summary>
    /// Check if key exists in cache.
    /// </summary>
    /// <param name="key">The key of the cached value to be searched.</param>
    /// <returns>Return a value that represents existence of the given key.</returns>
    public bool Contains(string key)
    {
        return _cache.ContainsKey(key);
    }

    /// <summary>
    /// Check if key exists in cache asynchronously.
    /// </summary>
    /// <param name="key">The key of the cached value to be searched.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Return a value that represents existence of the given key.</returns>
    public Task<bool> ContainsAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Contains(key));
    }

    /// <summary>
    /// Clears all cached items.
    /// </summary>
    public void Clear()
    {
        _cache.Clear();
    }

    /// <summary>
    /// Asynchronously clears all cached items.
    /// </summary>
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await Task.Run(Clear, cancellationToken);
    }

    /// <summary>
    /// Removes expired items from cache
    /// </summary>
    public async Task RemoveExpiredItemsAsync()
    {
        var expiredItems = _cache
            .Where(kvp =>
            {
                // Use reflection to check if the value is a CacheItem<T> and IsExpired is true
                var type = kvp.Value?.GetType();
                if (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(CacheItem<>))
                {
                    var isExpiredProperty = type.GetProperty("IsExpired");
                    if (isExpiredProperty != null)
                    {
                        var isExpiredValue = isExpiredProperty.GetValue(kvp.Value) as bool?;
                        return isExpiredValue == true;
                    }
                }
                return false;
            })
            .Select(kvp => kvp.Key)
            .ToList();

        // Remove expired items
        foreach (var key in expiredItems)
        {
            _cache.TryRemove(key, out _);
        }

        await Task.CompletedTask;
    }

    public MemoryCacheMetrics GetMetrics()
    {
        return _metrics;
    }
}