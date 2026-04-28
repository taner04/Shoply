using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Shoply.WebApi.Common.Infrastructure.Persistence;
using Shoply.WebApi.Features.Products.Models;

namespace Shoply.MigrationService;

public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
    : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private readonly ActivitySource _sActivitySource = new(ActivitySourceName);


    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var activity = _sActivitySource.StartActivity(ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ShoplyDbContext>();

            await RunMigrationAsync(dbContext, stoppingToken);
            await SeedDatabaseAsync(dbContext, stoppingToken);
        }
        catch (Exception e)
        {
            activity?.AddException(e);
            throw;
        }
        finally
        {
            applicationLifetime.StopApplication();
        }
    }

    private static async Task RunMigrationAsync(
        ShoplyDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () => { await dbContext.Database.MigrateAsync(cancellationToken); });
    }

    private static async Task SeedDatabaseAsync(ShoplyDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            var products = new[]
            {
                Product.Create(
                    "Wireless Bluetooth Headphones",
                    79.99m,
                    "High-quality wireless headphones with active noise cancellation, 30-hour battery life, and premium sound quality.",
                    45,
                    "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=500"
                ),
                Product.Create(
                    "Stainless Steel Coffee Maker",
                    49.99m,
                    "12-cup programmable coffee maker with thermal carafe, brew pause feature, and auto-shutoff for convenience.",
                    32,
                    "https://images.unsplash.com/photo-1517668808822-9ebb02ae2a0e?w=500"
                ),
                Product.Create(
                    "Organic Green Tea Set",
                    24.99m,
                    "Premium organic green tea with ceramic infuser set. Includes 50 servings of high-grade loose leaf tea.",
                    67,
                    "https://images.unsplash.com/photo-1597318810662-9868f5c0a0bf?w=500"
                ),
                Product.Create(
                    "Yoga Mat - Non-Slip",
                    35.99m,
                    "Extra thick 6mm exercise yoga mat with non-slip surface, carrying strap, and moisture-resistant design.",
                    58,
                    "https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=500"
                ),
                Product.Create(
                    "USB-C Fast Charger 65W",
                    39.99m,
                    "High-speed 65W USB-C power adapter compatible with laptops, phones, and tablets. Supports fast charging.",
                    90,
                    "https://images.unsplash.com/photo-1609091839311-d5365f9ff1c5?w=500"
                ),
                Product.Create(
                    "Mechanical Gaming Keyboard RGB",
                    129.99m,
                    "Professional mechanical keyboard with customizable RGB lighting, mechanical switches, and programmable keys.",
                    28,
                    "https://images.unsplash.com/photo-1587829191301-34f0e70f5d37?w=500"
                ),
                Product.Create(
                    "Non-Stick Cookware Set 10-Piece",
                    89.99m,
                    "Complete cookware set including frying pans, sauce pans, and cooking utensils. Oven-safe up to 400°F.",
                    41,
                    "https://images.unsplash.com/photo-1584568694244-14fbdf83bd30?w=500"
                ),
                Product.Create(
                    "Compact Desk Lamp with USB Charger",
                    22.99m,
                    "LED desk lamp with integrated USB charging port, three brightness levels, and flexible design.",
                    103,
                    "https://images.unsplash.com/photo-1565636192335-14f69814f7ce?w=500"
                ),
                Product.Create(
                    "Portable Bluetooth Speaker Waterproof",
                    59.99m,
                    "IPX7 waterproof portable speaker with 360° sound, 20-hour battery life, and rugged outdoor design.",
                    55,
                    "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=500"
                ),
                Product.Create(
                    "Natural Bamboo Cutting Board Set",
                    28.99m,
                    "Set of three eco-friendly bamboo cutting boards in different sizes with juice grooves and handles.",
                    76,
                    "https://images.unsplash.com/photo-1607623814075-e51df1bdc82f?w=500"
                )
            };

            foreach (var product in products)
            {
                product.SetCreated();
                dbContext.Products.Add(product);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        });
    }
}