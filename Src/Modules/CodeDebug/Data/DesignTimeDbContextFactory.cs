using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CodeDebug.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CodeDebugDbContext>
{
    public CodeDebugDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<CodeDebugDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=codedebug;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new CodeDebugDbContext(builder.Options);
    }
}