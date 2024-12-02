using Cachey.Core;
using Microsoft.Extensions.Logging;

namespace Cachey.Tests;

public class CacheCleanupServiceTests
{
    private readonly CacheCleanupService _cleanupService;
    private readonly MemoryCache _memoryCache;

    public CacheCleanupServiceTests()
    {
        _memoryCache = new MemoryCache();
        var logger = new Logger<CacheCleanupService>(new LoggerFactory());
        _cleanupService = new CacheCleanupService(logger, _memoryCache, TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public async Task CleanupService_Should_Remove_Expired_Items()
    {
        // Arrange
        await _memoryCache.SetAsync("key1", "value1", TimeSpan.FromMilliseconds(50));
        await _memoryCache.SetAsync("key2", "value2", TimeSpan.FromMilliseconds(250));

        // Act
        await _cleanupService.StartAsync(CancellationToken.None);
        await Task.Delay(200); // Wait for cleanup to execute

        // Assert
        Assert.False(await _memoryCache.ContainsAsync("key1")); // Should be removed
        Assert.True(await _memoryCache.ContainsAsync("key2")); // Should still exist

        await _cleanupService.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task CleanupService_Should_Not_Remove_Unexpired_Items()
    {
        // Arrange
        await _memoryCache.SetAsync("key1", "value1", TimeSpan.FromSeconds(1));
        await _memoryCache.SetAsync("key2", "value2", TimeSpan.FromSeconds(2));

        // Act
        await _cleanupService.StartAsync(CancellationToken.None);
        await Task.Delay(500); // Wait for cleanup

        // Assert
        Assert.True(await _memoryCache.ContainsAsync("key1")); // Should still exist
        Assert.True(await _memoryCache.ContainsAsync("key2")); // Should still exist

        await _cleanupService.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task CleanupService_Should_Handle_Empty_Cache()
    {
        // Act
        await _cleanupService.StartAsync(CancellationToken.None);
        await Task.Delay(200); // Wait for cleanup to execute

        // Assert
        Assert.True(true); // No exceptions should occur

        await _cleanupService.StopAsync(CancellationToken.None);
    }
}