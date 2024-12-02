namespace Cachey.Persistence.SQLite;

public class CacheItemEntity
{
    /// <summary>
    /// Cache key (cache identifier)
    /// </summary>
    public string Key { get; set; }
    /// <summary>
    /// Cache value that will be serialized to string (must be deserialized during the read)
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// Creation time
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Expiration time
    /// </summary>
    public DateTime? Expiration { get; set; } // Време истека кеша
}