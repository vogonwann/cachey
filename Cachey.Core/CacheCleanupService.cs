using Cachey.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cachey.Core;

public class CacheCleanupService(ILogger<CacheCleanupService> logger, ICache cache) : BackgroundService
{
    private TimeSpan _cleanupInterval = TimeSpan.FromDays(1);

    public void SetCleanupInterval(TimeSpan interval)
    {
        _cleanupInterval = interval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Cache cleanup service started with interval {Interval}", _cleanupInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Running cache cleanup...");
                await cache.RemoveExpiredItemsAsync();
                logger.LogInformation("Cache cleanup completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during cache cleanup.");
            }

            await Task.Delay(_cleanupInterval, stoppingToken);
        }

        logger.LogInformation("Cache cleanup service stopping.");
    }
}