using Microsoft.EntityFrameworkCore.Storage;
using Shoply.WebApi.Common.Infrastructure.Persistence.Configurations;
using Shoply.WebApi.Features.WebHooks.Models;

namespace Shoply.WebApi.Common.Infrastructure.Persistence;

public sealed class ShoplyDbContext(DbContextOptions<ShoplyDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<WebHookEvent> WebHookEvents => Set<WebHookEvent>();

    // Query Side - IQueryable (AsNoTracking optimized)
    public IQueryable<Product> ProductsQuery =>
        Set<Product>().AsNoTracking();

    public IQueryable<User> UsersQuery =>
        Set<User>().AsNoTracking();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShoplyDbContext).Assembly);
    }

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.RegisterAllInEfcVogenIdConverter();
    }
}