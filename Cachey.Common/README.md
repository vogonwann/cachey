
# Cachey

**Cachey** is a lightweight, flexible caching library for .NET applications. It supports both in-memory and persistent caching with easy integration and customization.

## Features

- In-memory caching for fast and lightweight operations.
- Persistent caching for durable data storage.
- Automatic cleanup of expired items with customizable intervals.
- Seamless integration with dependency injection (DI) and configuration.

## Installation

Add **Cachey** to your project using your package manager:

```bash
dotnet add package Cachey
```

## Quick Start

### 1. Register Cachey in DI

You can register `Cachey` in your application's DI container using the `UseCachey` method. Here's an example:

```csharp
builder.Services.UseCachey(builder.Configuration)
    .AddCacheCleanupService(TimeSpan.FromHours(1));
```

- `UseCachey` integrates Cachey into your application, dynamically deciding whether to use in-memory or persistent cache based on configuration.
- `AddCacheCleanupService` registers a background service to periodically clean up expired cache items.

### 2. Configuring Persistence

The caching behavior is determined by the `Cache` section in your application's configuration file (e.g., `appsettings.json`).

#### Example Configuration:

```json
{
  "Cache": {
    "UsePersistentCache": true
  }
}
```

- Set `UsePersistentCache` to `true` to enable persistent caching.
- When persistent caching is enabled, Cachey will use a SQLite database to store cache entries.

#### Using a Custom Connection String:

To specify a custom SQLite connection string, add the following to your configuration:

```json
{
  "Cache": {
    "UsePersistentCache": true,
    "ConnectionString": "DataSource=mycustomcache.db"
  }
}
```

> Cachey automatically configures a default SQLite database (`mycache.db`) if a custom connection string is not provided.

### 3. Accessing Cachey in Your Code

Inject the `ICache` interface into your services or controllers to start using Cachey:

```csharp
public class MyService
{
    private readonly ICache _cache;

    public MyService(ICache cache)
    {
        _cache = cache;
    }

    public async Task DoWorkAsync()
    {
        // Store a value in the cache
        await _cache.SetAsync("key", "value", TimeSpan.FromMinutes(10));

        // Retrieve a value from the cache
        var cachedValue = await _cache.GetAsync<string>("key");

        // Remove a value from the cache
        await _cache.RemoveAsync("key");
    }
}
```

## Adding Cleanup Service

The cleanup service automatically removes expired cache items at regular intervals. You can customize the interval like this:

```csharp
builder.Services.UseCachey(builder.Configuration)
    .AddCacheCleanupService(TimeSpan.FromMinutes(30));
```

### Cleanup Behavior

The cleanup service runs periodically, based on the specified interval, to ensure expired items are removed from the cache.

## Full Example

Here’s a complete example of setting up Cachey in an ASP.NET Core application:

### `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Cachey
builder.Services.UseCachey(builder.Configuration)
    .AddCacheCleanupService(TimeSpan.FromHours(1));

var app = builder.Build();

app.MapGet("/", async (ICache cache) =>
{
    // Set a cache entry
    await cache.SetAsync("example", "Hello, Cachey!", TimeSpan.FromMinutes(5));

    // Retrieve the cached value
    var value = await cache.GetAsync<string>("example");
    return value ?? "Cache entry not found!";
});

app.Run();
```

### `appsettings.json`

```json
{
  "Cache": {
    "UsePersistentCache": true,
    "ConnectionString": "DataSource=mycustomcache.db"
  }
}
```

---

## License

Cachey is licensed under the MIT License.
