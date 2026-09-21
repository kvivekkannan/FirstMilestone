using FirstMilestone.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FirstMilestone.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.DashboardModules.AnyAsync()) return;

        db.DashboardModules.AddRange(
            new DashboardModule
            {
                Key = "weather",
                Name = "Weather",
                Description = "Current conditions and a short forecast powered by Open-Meteo.",
                Icon = "☀",
                IsInstalled = true
            },
            new DashboardModule
            {
                Key = "stocks",
                Name = "Stocks",
                Description = "Reserved module for stock watchlists and charts.",
                Icon = "↗",
                IsInstalled = false
            },
            new DashboardModule
            {
                Key = "news",
                Name = "News",
                Description = "Reserved module for a configurable news feed.",
                Icon = "▤",
                IsInstalled = false
            });

        await db.SaveChangesAsync();
    }
}
