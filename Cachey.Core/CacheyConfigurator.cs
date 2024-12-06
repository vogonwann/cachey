using Cachey.Common.Interfaces;
using Cachey.Persistence.SQLite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cachey.Core;

public static class CacheyConfigurator
{
    public static IServiceCollection UseCachey(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        bool.TryParse(configuration.GetSection("Cache").Value, out var usePersistentCache);
        
        if (usePersistentCache)
        {
            serviceCollection.AddDbContext<CacheyDbContext>(options =>
                options.UseSqlite("DataSource=mycache.db"));
            serviceCollection.AddSingleton<ICache, SqliteCacheRepository>();
        }
        else
        {
            serviceCollection.AddSingleton<ICache, MemoryCache>();
        }

        return serviceCollection;
    }
    public static IServiceCollection AddCacheCleanupService(this IServiceCollection services, TimeSpan cleanupInterval)
    {
        services.AddSingleton<CacheCleanupService>();
        services.AddHostedService(provider =>
        {
            var cacheCleanupService = provider.GetService<CacheCleanupService>();
            cacheCleanupService?.SetCleanupInterval(cleanupInterval);
            return cacheCleanupService;
        });

        return services;
    }
}