using System.Text.Json;
using Cachey.Core;
using Microsoft.EntityFrameworkCore;

namespace Cachey.Persistence.SQLite;

public class SqliteCacheRepository(CacheyDbContext dbContext) : ICache
{
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null,
        CancellationToken cancellationToken = default)
    {
        var cacheItem = new CacheItem<T>(value, expiry); // Пример времена истека
        var entity = new CacheItemEntity
        {
            Key = key,
            Value = JsonSerializer.Serialize(cacheItem), // Серијализација објекта
            CreatedAt = cacheItem.CreatedAt,
            Expiration = cacheItem.ExpirationTime
        };

        await dbContext.CacheItems.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string key, TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.CacheItems
            .FirstOrDefaultAsync(x => x.Key == key, cancellationToken);

        if (entity == null) return default;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new CacheItemConverter());
        var cacheItem = JsonSerializer.Deserialize<CacheItem<T>>(entity.Value, options); // Deserialize the item

        // Explicitly cast to nullable type T? and return the value
        return cacheItem != null ? (T?)cacheItem.Value : default;
    }

    public void Set<T>(string key, T value, TimeSpan? expiry = null)
    {
        var cacheItem = new CacheItem<T>(value, expiry); // Пример времена истека
        var entity = new CacheItemEntity
        {
            Key = key,
            Value = JsonSerializer.Serialize(cacheItem), // Серијализација објекта
            CreatedAt = cacheItem.CreatedAt,
            Expiration = cacheItem.ExpirationTime
        };

        dbContext.CacheItems.Add(entity);
        dbContext.SaveChanges();
    }

    public T? Get<T>(string key)
    {
        var entity = dbContext.CacheItems
            .FirstOrDefault(x => x.Key == key);

        if (entity == null) return default;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new CacheItemConverter());
        var cacheItem = JsonSerializer.Deserialize<CacheItem<T>>(entity.Value, options); // Deserialize the item

        // Explicitly cast to nullable type T? and return the value
        return cacheItem != null ? (T?)cacheItem.Value : default;
    }

    public void Remove(string key)
    {
        var entity = dbContext.CacheItems
            .FirstOrDefault(x => x.Key == key);

        if (entity != null)
        {
            dbContext.CacheItems.Remove(entity);
            dbContext.SaveChanges();
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Remove(key);

        await Task.CompletedTask;
    }

    public bool Contains(string key)
    {
        return dbContext.CacheItems.Any(x => x.Key == key);
    }

    public async Task<bool> ContainsAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await Task.FromResult(Contains(key));
    }

    public void Clear()
    {
        dbContext.CacheItems.RemoveRange(dbContext.CacheItems);
        dbContext.SaveChanges();
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        dbContext.CacheItems.RemoveRange(dbContext.CacheItems);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveExpiredItemsAsync()
    {
        var expiredItems = await dbContext.CacheItems
            .Where(x => x.Expiration < DateTime.Now)
            .ToListAsync();

        dbContext.CacheItems.RemoveRange(expiredItems);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(string key)
    {
        var entity = await dbContext.CacheItems
            .FirstOrDefaultAsync(x => x.Key == key);

        if (entity != null)
        {
            dbContext.CacheItems.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<bool> ContainsAsync(string containskey)
    {
        return await dbContext.CacheItems.AnyAsync(i => i.Key == containskey);
    }

    public async Task ClearAsync()
    {
        dbContext.CacheItems.RemoveRange(dbContext.CacheItems);
        await dbContext.SaveChangesAsync();
    }
}