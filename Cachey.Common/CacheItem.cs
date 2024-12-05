namespace Cachey.Common;

public class CacheItem<T>
{
    public CacheItem(T value, TimeSpan? expirationTime)
    {
        Value = value;
        CreatedAt = DateTime.UtcNow;
        ExpirationTime = expirationTime.HasValue
            ? DateTime.UtcNow.Add(expirationTime.Value)
            : DateTime.MaxValue;
    }

    public T? Value { get; }
    public DateTime? ExpirationTime { get; }
    public DateTime CreatedAt { get; set; }
    public bool IsExpired => ExpirationTime.HasValue && ExpirationTime.Value < DateTime.UtcNow;
}