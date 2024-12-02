using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cachey.Core;

public class CacheCleanupService : BackgroundService
{
    private readonly ICache _cache;
    private readonly ILogger<CacheCleanupService> _logger;
    private TimeSpan _cleanupInterval;

    public CacheCleanupService(ILogger<CacheCleanupService> logger, ICache cacheRepository, TimeSpan cleanupInterval)
    {
        _logger = logger;
        _cache = cacheRepository;
        _cleanupInterval = cleanupInterval; // Подразумевани интервал
    }

    public void SetCleanupInterval(TimeSpan interval)
    {
        _cleanupInterval = interval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cache cleanup service started with interval {Interval}", _cleanupInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Running cache cleanup...");
                await _cache.RemoveExpiredItemsAsync();
                _logger.LogInformation("Cache cleanup completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cache cleanup.");
            }

            await Task.Delay(_cleanupInterval, stoppingToken);
        }

        _logger.LogInformation("Cache cleanup service stopping.");
    }
}