using FirstMilestone.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FirstMilestone.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<DashboardModule> DashboardModules => Set<DashboardModule>();
    public DbSet<WeatherSearch> WeatherSearches => Set<WeatherSearch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DashboardModule>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Key).IsUnique();
            entity.Property(x => x.Key).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Icon).HasMaxLength(10);
        });

        modelBuilder.Entity<WeatherSearch>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.City).HasMaxLength(160).IsRequired();
            entity.HasIndex(x => x.SearchedUtc);
        });
    }
}
