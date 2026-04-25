using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CodeDebug.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CodeDebugDbContext>
{
    public CodeDebugDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<CodeDebugDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5431;Database=codedebug_modular_monolith;User Id=postgres;Password=changeme;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new CodeDebugDbContext(builder.Options);
    }
}
