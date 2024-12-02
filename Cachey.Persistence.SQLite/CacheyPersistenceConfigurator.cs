using Cachey.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cachey.Persistence.SQLite;

public static class CacheyPersistenceConfigurator
{
    public static IServiceCollection UseCacheyPersistence(this IServiceCollection serviceCollection,
        string connectionString)
    {
        serviceCollection.AddDbContext<CacheyDbContext>(options => options.UseSqlite(connectionString));
        serviceCollection.AddScoped<ICache, SqliteCacheRepository>();
        return serviceCollection;
    }
}