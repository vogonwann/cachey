using Microsoft.EntityFrameworkCore;

namespace Cachey.Persistence.SQLite;

public class CacheyDbContext(DbContextOptions<CacheyDbContext> options) : DbContext(options)
{
    public DbSet<CacheItemEntity> CacheItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CacheItemEntity>().ToTable("CacheItems");


        modelBuilder.Entity<CacheItemEntity>()
            .HasKey(x => x.Key); // Key is the index

        modelBuilder.Entity<CacheItemEntity>()
            .Property(x => x.Key)
            .IsRequired();

        modelBuilder.Entity<CacheItemEntity>()
            .Property(x => x.Value)
            .IsRequired();

        modelBuilder.Entity<CacheItemEntity>()
            .Property(x => x.Expiration)
            .IsRequired();

        modelBuilder.Entity<CacheItemEntity>()
            .Property(x => x.CreatedAt)
            .IsRequired();
    }
}