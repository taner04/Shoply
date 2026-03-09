using Api.Common.Domain.Baskets;
using Api.Common.Domain.Orders;
using Api.Common.Infrastructure.Persistence.Configurations;

namespace Api.Common.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    // Aggregates
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Basket> Baskets => Set<Basket>();


    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.RegisterAllInEfcVogenIdConverter();
    }
}