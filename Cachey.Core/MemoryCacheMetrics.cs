namespace Cachey.Core;

/// <summary>
///     Class to track cache usage metrics like requests, hits, and misses.
/// </summary>
public class MemoryCacheMetrics
{
    public int TotalRequests { get; private set; }
    public int CacheHits { get; private set; }
    public int CacheMisses => TotalRequests - CacheHits;

    public void RegisterHit()
    {
        TotalRequests++;
        CacheHits++;
    }

    public void RegisterMiss()
    {
        TotalRequests++;
    }

    public override string ToString()
    {
        return $"Requests: {TotalRequests}, Hits: {CacheHits}, Misses: {CacheMisses}";
    }
}