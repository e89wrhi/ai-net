using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityContext>
{
    public IdentityContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<IdentityContext>();

        builder.UseNpgsql("Server=localhost;Port=5431;Database=identity_modular_monolith;User Id=postgres;Password=changeme;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new IdentityContext(builder.Options);
    }
}
