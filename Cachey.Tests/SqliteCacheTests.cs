using Cachey.Core;
using Cachey.Persistence.SQLite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cachey.Tests;

public class SqliteCacheTests
{
    private readonly CacheyDbContext _context;
    private readonly SqliteCacheRepository _sqliteCache;

    public SqliteCacheTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<CacheyDbContext>()
            .UseSqlite(connection)
            .Options;

        _context = new CacheyDbContext(options);

        _sqliteCache = new SqliteCacheRepository(_context);

        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task SetAndGetAsync_Should_Store_And_Retrieve_Value()
    {
        // Arrange
        const string key = "testKey";
        const string value = "testValue";

        // Act
        await _sqliteCache.SetAsync(key, value, TimeSpan.FromMinutes(10)); // Добавите Expiration
        var retrievedValue = await _sqliteCache.GetAsync<string>(key);

        // Assert
        Assert.Equal(value, retrievedValue);
    }

    [Fact]
    public async Task GetAsync_Should_Return_Null_For_Expired_Item()
    {
        // Arrange
        const string key = "expiredKey";
        const string value = "expiredValue";
        
        // Start background service
        var logger = new Logger<CacheCleanupService>(new LoggerFactory());
        var cleanupService = new CacheCleanupService(logger, _sqliteCache);
        cleanupService.SetCleanupInterval(TimeSpan.FromMilliseconds(10));
        var cancellationTokenSource = new CancellationTokenSource();
        var cleanupTask = cleanupService.StartAsync(cancellationTokenSource.Token);

        // Wait for the service to remove the expired items
        await Task.Delay(50);

        await _sqliteCache.SetAsync(key, value, TimeSpan.FromMilliseconds(5), cancellationTokenSource.Token);
        await Task.Delay(100, cancellationTokenSource.Token); // Wait for expiration
        
        // Stop the background service
        cancellationTokenSource.Cancel();
        await cleanupTask;

        // Act
        var retrievedValue = await _sqliteCache.GetAsync<string>(key);

        // Assert
        Assert.Null(retrievedValue);
    }

    [Fact]
    public async Task RemoveAsync_Should_Remove_Stored_Item()
    {
        // Arrange
        const string key = "removeKey";
        const string value = "removeValue";

        await _sqliteCache.SetAsync(key, value, TimeSpan.FromMinutes(10));

        // Act
        await _sqliteCache.RemoveAsync(key);
        var retrievedValue = await _sqliteCache.GetAsync<string>(key);

        // Assert
        Assert.Null(retrievedValue);
    }

    [Fact]
    public async Task ContainsAsync_Should_Return_True_If_Item_Exists()
    {
        // Arrange
        const string key = "containsKey";
        const string value = "containsValue";

        await _sqliteCache.SetAsync(key, value, TimeSpan.FromMinutes(10));

        // Act
        var exists = await _sqliteCache.ContainsAsync(key);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ContainsAsync_Should_Return_False_If_Item_Does_Not_Exist()
    {
        // Act
        var exists = await _sqliteCache.ContainsAsync("nonExistentKey");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task ClearAsync_Should_Remove_All_Items()
    {
        // Arrange
        await _sqliteCache.SetAsync("key1", "value1", TimeSpan.FromMinutes(10));
        await _sqliteCache.SetAsync("key2", "value2", TimeSpan.FromMinutes(10));

        // Act
        await _sqliteCache.ClearAsync();

        // Assert
        Assert.False(await _sqliteCache.ContainsAsync("key1"));
        Assert.False(await _sqliteCache.ContainsAsync("key2"));
    }
}