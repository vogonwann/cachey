namespace Cachey.Core;

/// <summary>
///     Interface for caching, which can be implemented in different caching types.
/// </summary>
public interface ICache
{
    T? Get<T>(string key);
    Task<T?> GetAsync<T>(string key, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    void Set<T>(string key, T value, TimeSpan? expiration = null);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration, CancellationToken cancellationToken = default);
    void Remove(string key);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    bool Contains(string key);
    Task<bool> ContainsAsync(string key, CancellationToken cancellationToken = default);
    void Clear();
    Task ClearAsync(CancellationToken cancellationToken = default);
    Task RemoveExpiredItemsAsync();
}