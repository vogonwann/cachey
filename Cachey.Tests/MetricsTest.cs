using Cachey.Core;

namespace Cachey.Tests;

public class MetricsTest
{
    [Fact]
    public void Metrics_Should_Start_With_Zero_Values()
    {
        // Arrange
        var cache = new MemoryCache();

        // Act
        var metrics = cache.GetMetrics();

        // Assert
        Assert.Equal(0, metrics.TotalRequests);
        Assert.Equal(0, metrics.CacheHits);
        Assert.Equal(0, metrics.CacheMisses);
    }

    [Fact]
    public void Get_Should_Update_Metrics_On_Hit()
    {
        // Arrange
        var cache = new MemoryCache();
        cache.Set("testKey", "testValue");

        // Act
        var value = cache.Get<string>("testKey");

        // Assert
        Assert.Equal("testValue", value);
        var metrics = cache.GetMetrics();
        Assert.Equal(1, metrics.CacheHits);
        Assert.Equal(1, metrics.TotalRequests);
        Assert.Equal(0, metrics.CacheMisses);
    }

    [Fact]
    public void Get_Should_Update_Metrics_On_Miss()
    {
        // Arrange
        var cache = new MemoryCache();

        // Act
        var value = cache.Get<string>("nonExistentKey");

        // Assert
        Assert.Null(value);
        var metrics = cache.GetMetrics();
        Assert.Equal(0, metrics.CacheHits);
        Assert.Equal(1, metrics.TotalRequests);
        Assert.Equal(1, metrics.CacheMisses);
    }

    [Fact]
    public void Get_Should_Update_Metrics_On_Multiple_Requests()
    {
        // Arrange
        var cache = new MemoryCache();
        cache.Set("testKey", "testValue");

        // Act
        var hitValue = cache.Get<string>("testKey"); // Hit
        var missValue = cache.Get<string>("nonExistentKey"); // Miss

        // Assert
        Assert.Equal("testValue", hitValue);
        Assert.Null(missValue);

        var metrics = cache.GetMetrics();
        Assert.Equal(2, metrics.TotalRequests);
        Assert.Equal(1, metrics.CacheHits);
        Assert.Equal(1, metrics.CacheMisses);
    }

    [Fact]
    public async Task GetAsync_Should_Update_Metrics_On_Hit()
    {
        // Arrange
        var cache = new MemoryCache();
        await cache.SetAsync("testKey", "testValue");

        // Act
        var value = await cache.GetAsync<string>("testKey");

        // Assert
        Assert.Equal("testValue", value);
        var metrics = cache.GetMetrics();
        Assert.Equal(1, metrics.CacheHits);
        Assert.Equal(1, metrics.TotalRequests);
        Assert.Equal(0, metrics.CacheMisses);
    }

    [Fact]
    public async Task GetAsync_Should_Update_Metrics_On_Miss()
    {
        // Arrange
        var cache = new MemoryCache();

        // Act
        var value = await cache.GetAsync<string>("nonExistentKey");

        // Assert
        Assert.Null(value);
        var metrics = cache.GetMetrics();
        Assert.Equal(0, metrics.CacheHits);
        Assert.Equal(1, metrics.TotalRequests);
        Assert.Equal(1, metrics.CacheMisses);
    }

    [Fact]
    public async Task GetAsync_Should_Update_Metrics_On_Multiple_Requests()
    {
        // Arrange
        var cache = new MemoryCache();
        await cache.SetAsync("testKey", "testValue");

        // Act
        var hitValue = await cache.GetAsync<string>("testKey"); // Hit
        var missValue = await cache.GetAsync<string>("nonExistentKey"); // Miss

        // Assert
        Assert.Equal("testValue", hitValue);
        Assert.Null(missValue);

        var metrics = cache.GetMetrics();
        Assert.Equal(2, metrics.TotalRequests);
        Assert.Equal(1, metrics.CacheHits);
        Assert.Equal(1, metrics.CacheMisses);
    }
}