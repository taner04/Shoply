using Microsoft.EntityFrameworkCore.Design;
using ServiceDefaults;

namespace Api.Common.Infrastructure.Persistence;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseNpgsql(AppHostConstants.Database);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}