using Api.Common.Infrastructure.Persistence.Configurations;

namespace Api.Common.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    // Command Side - DbSet (tracked)
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();

    // Query Side - IQueryable (AsNoTracking optimized)
    public IQueryable<Product> ProductsQuery =>
        Set<Product>().AsNoTracking();

    public IQueryable<User> UsersQuery =>
        Set<User>().AsNoTracking();

    public IQueryable<Order> OrdersQuery =>
        Set<Order>().AsNoTracking();


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