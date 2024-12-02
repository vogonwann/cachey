using Microsoft.Extensions.DependencyInjection;

namespace Cachey.Core;

public static class CacheyConfigurator
{
    public static IServiceCollection UseCachey(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ICache, MemoryCache>();
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