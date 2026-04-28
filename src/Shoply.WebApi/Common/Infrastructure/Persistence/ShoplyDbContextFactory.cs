using Microsoft.EntityFrameworkCore.Design;
using Shoply.ServiceDefaults;

namespace Shoply.WebApi.Common.Infrastructure.Persistence;

public sealed class ShoplyDbContextFactory : IDesignTimeDbContextFactory<ShoplyDbContext>
{
    public ShoplyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShoplyDbContext>();
        optionsBuilder.UseNpgsql(AppHostConstants.Database);

        return new ShoplyDbContext(optionsBuilder.Options);
    }
}