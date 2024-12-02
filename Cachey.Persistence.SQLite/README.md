
# Cachey

**Cachey** is a lightweight, modular, and extensible caching library designed to provide in-memory and persistent caching mechanisms with configurable expiration and cleanup policies.

## Features

- **In-Memory Cache**: High-performance caching for frequently accessed data.
- **Persistent Cache**: Optional persistent storage with SQLite support.
- **Expiration Policies**: Configurable item expiration times.
- **Background Cleanup**: Periodic removal of expired items for optimal memory usage.
- **Metrics**: Tracks cache hits, misses, and overall performance.

## Installation

Cachey is distributed as a .NET library. To install, use the NuGet package manager:

```bash
dotnet add package Cachey
```

## Usage

### Basic Setup

To start using Cachey, initialize an instance of the `Cache` class:

```csharp
var cache = new Cache();
```

### Adding and Retrieving Items

```csharp
await cache.SetAsync("key", "value", TimeSpan.FromMinutes(5));
var value = await cache.GetAsync<string>("key");
```

### Checking for Existence

```csharp
bool exists = await cache.ContainsAsync("key");
```

### Removing Items

```csharp
await cache.RemoveAsync("key");
```

### Clearing the Cache

```csharp
await cache.ClearAsync();
```

### Background Cleanup Service

Cachey includes a background cleanup service to remove expired items automatically. You can configure and start it as follows:

```csharp
var cleanupService = new BackgroundCleanupService(cache, TimeSpan.FromSeconds(30));
cleanupService.Start();
```

## Metrics

Metrics provide insights into cache performance:

```csharp
var metrics = cache.GetMetrics();
Console.WriteLine($"Hits: {metrics.Hits}, Misses: {metrics.Misses}");
```

## Persistent Caching

To enable persistent caching with SQLite:

1. Add the `Cachey.Persistence.SQLite` package:
   ```bash
   dotnet add package Cachey.Persistence.SQLite
   ```

2. Use the `SqliteCacheRepository`:

   ```csharp
   var options = new DbContextOptionsBuilder<CacheyDbContext>()
       .UseSqlite("Data Source=cachey.db")
       .Options;

   var context = new CacheyDbContext(options);
   var persistentCache = new SqliteCacheRepository(context);

   var cache = new Cache(persistentCache);
   ```

## Testing

Cachey includes a suite of tests to ensure reliability. To run the tests, use:

```bash
dotnet test
```

## Contributions

Contributions are welcome! Feel free to submit issues or pull requests on the [GitHub repository](https://github.com/your-repo/cachey).

## License

Cachey is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
