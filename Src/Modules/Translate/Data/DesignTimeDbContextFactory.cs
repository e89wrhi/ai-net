using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Translate.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TranslateDbContext>
{
    public TranslateDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<TranslateDbContext>();

        builder.UseNpgsql("Server=localhost;Port=5432;Database=translate;User Id=postgres;Password=postgres;Include Error Detail=true")
            .UseSnakeCaseNamingConvention();
        return new TranslateDbContext(builder.Options);
    }
}