namespace Cachey.Core;

public class CacheOptions
{
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromMinutes(5);
}