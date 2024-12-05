using Cachey.Core;
using Microsoft.Extensions.Logging;

namespace Cachey.Tests;

public class MemoryCacheTests
{
    private readonly MemoryCache _cache = new();

    [Fact]
    public async Task GetAsync_Should_Return_Null_For_Expired_Items()
    {
        // Arrange
        const string key = "expiredKey";
        const string value = "expiredValue";
        await _cache.SetAsync(key, value, TimeSpan.FromMilliseconds(10));

        // Act
        await Task.Delay(20); // Чекамо да ставка истекне
        var retrievedValue = await _cache.GetAsync<string>(key);

        // Assert
        Assert.Null(retrievedValue);
        Assert.True(await _cache.ContainsAsync(key)); // Истекла ставка неће бити уклоњена
    }

    [Fact]
    public async Task RemoveExpiredItemsAsync_Should_Remove_Expired_Items()
    {
        // Arrange
        const string key1 = "expiredKey1";
        const string value1 = "expiredValue1";
        const string key2 = "validKey";
        const string value2 = "validValue";

        await _cache.SetAsync(key1, value1, TimeSpan.FromMilliseconds(10));
        await _cache.SetAsync(key2, value2, TimeSpan.FromSeconds(10));

        await Task.Delay(20); // Чекамо да прва ставка истекне

        // Act
        await _cache.RemoveExpiredItemsAsync();

        // Assert
        Assert.False(await _cache.ContainsAsync(key1)); // Истекла ставка је уклоњена
        Assert.True(await _cache.ContainsAsync(key2)); // Важећа ставка и даље постоји
    }

    [Fact]
    public async Task GetAsync_Should_Return_Value_For_NonExpired_Items()
    {
        // Arrange
        const string key = "validKey";
        const string value = "validValue";
        await _cache.SetAsync(key, value, TimeSpan.FromMilliseconds(50));

        // Act
        var retrievedValue = await _cache.GetAsync<string>(key);

        // Assert
        Assert.Equal(value, retrievedValue);
    }

    [Fact]
    public async Task BackgroundCleanup_Should_Periodically_Remove_Expired_Items()
    {
        // Arrange
        const string key1 = "expiredKey1";
        const string value1 = "expiredValue1";
        const string key2 = "validKey";
        const string value2 = "validValue";

        await _cache.SetAsync(key1, value1, TimeSpan.FromMilliseconds(10));
        await _cache.SetAsync(key2, value2, TimeSpan.FromSeconds(10));

        // Start background service
        var logger = new Logger<CacheCleanupService>(new LoggerFactory());
        var cleanupService = new CacheCleanupService(logger, _cache);
        cleanupService.SetCleanupInterval(TimeSpan.FromMilliseconds(10));
        var cancellationTokenSource = new CancellationTokenSource();
        var cleanupTask = cleanupService.StartAsync(cancellationTokenSource.Token);

        // Wait for the service to remove the expired items
        await Task.Delay(50);

        // Act
        var existsKey1 = await _cache.ContainsAsync(key1, cancellationTokenSource.Token);
        var existsKey2 = await _cache.ContainsAsync(key2, cancellationTokenSource.Token);

        // Stop the background seervice
        cancellationTokenSource.Cancel();
        await cleanupTask;

        // Assert
        Assert.False(existsKey1);
        Assert.True(existsKey2);
    }
    
    [Fact]
    public async Task RemoveExpiredItemsAsync_Should_Remove_Only_Expired_Items()
    {
        await _cache.SetAsync("key1", "value1", TimeSpan.FromMilliseconds(10));
        await _cache.SetAsync("key2", "value2", TimeSpan.FromSeconds(10));
    
        await Task.Delay(50); // Чекамо да прва ставка истекне

        // Act
        await _cache.RemoveExpiredItemsAsync();

        // Assert
        Assert.False(await _cache.ContainsAsync("key1")); // Прва је истекла
        Assert.True(await _cache.ContainsAsync("key2"));  // Друга је валидна
    }

}